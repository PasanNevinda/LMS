

/** 
* AjaxService - A robust AJAX utility for ASP.NET Core MVC
* Supports GET, POST, file upload, retry logic, and anti-forgery handling.
*/
class AjaxService {

    // === Configuration ===
    static globalErrorCallback = null;
    static debug = false;

    static autoRedirectOnAuthFailure = true;
    static setAutoRedirectOnAuthFailure(enabled) {
        this.autoRedirectOnAuthFailure = !!enabled;
    }


    static defaultRetryConfig = {
        maxRetries: 3,
        retryDelay: 1000, // ms
        retryMultiplier: 2, // exponential backoff
        retryableErrors: ['NetworkError', 'TimeoutError', 'AbortError']
    };

    // === Setup methods ===
    static setGlobalErrorHandler(callback) {
        this.globalErrorCallback = callback;
    }

    static setRetryConfig(config) {
        this.defaultRetryConfig = { ...this.defaultRetryConfig, ...config };
    }

    static setDebug(enabled) {
        this.debug = !!enabled;
    }

    static _log(...args) {
        if (this.debug) console.log(...args);
    }

    // === Public GET ===
    static async get(url, params = {}, options = {}) {
        const config = {
            timeout: 30000,
            headers: {},
            retry: this.defaultRetryConfig,
            suppressGlobalError: false,
            ...options,
        };
        config.retry = { ...this.defaultRetryConfig, ...options.retry };

        return this._executeWithRetry(
            () => this._performGet(url, params, config),
            config.retry,
            'GET',
            url,
            config.suppressGlobalError
        );
    }

    static async _performGet(url, params = {}, config = {}) {
        if (!url || typeof url !== 'string')
            throw new Error('URL is required and must be a string');

        const queryString = this._buildQueryString(params);
        const fullUrl = queryString ? `${url}${url.includes('?') ? '&' : '?'}${queryString}` : url;

        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), config.timeout);

        const headers = {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/json',
            ...config.headers
        };

        const token = this._getAntiForgeryToken();
        if (token) headers['RequestVerificationToken'] = token;

        this._log(`GET Request: ${fullUrl}`);

        try {
            const response = await fetch(fullUrl, {
                method: 'GET',
                headers,
                signal: controller.signal,
                credentials: 'same-origin'
            });

            return await this._handleResponse(response, 'GET', fullUrl);
        } catch (error) {
            throw error;
        } finally {
            clearTimeout(timeoutId);
        }
    }

    // === Public POST ===
    static async post(url, data = {}, options = {}) {
        const config = {
            timeout: 30000,
            headers: {},
            isFormData: data instanceof FormData,
            retry: this.defaultRetryConfig,
            suppressGlobalError: false,
            ...options
        };
        config.retry = { ...this.defaultRetryConfig, ...options.retry };

        return this._executeWithRetry(
            () => this._performPost(url, data, config),
            config.retry,
            'POST',
            url,
            config.suppressGlobalError
        );
    }

    static async _performPost(url, data = {}, config = {}) {
        if (!url || typeof url !== 'string')
            throw new Error('URL is required and must be a string');

        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), config.timeout);

        const headers = { 'X-Requested-With': 'XMLHttpRequest', ...config.headers };
        const token = this._getAntiForgeryToken();

        let body;

        if (config.isFormData) {
            if (token && !data.has('__RequestVerificationToken')) {
                data.append('__RequestVerificationToken', token);
            }
            body = data;
        } else {
            headers['Content-Type'] = 'application/json';
            body = JSON.stringify(data);
            if (token) headers['RequestVerificationToken'] = token;
        }

        this._log(`POST Request: ${url}`, config.isFormData ? '[FormData]' : data);

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers,
                body,
                signal: controller.signal,
                credentials: 'same-origin'
            });

            return await this._handleResponse(response, 'POST', url);
        } catch (error) {
            throw error;
        } finally {
            clearTimeout(timeoutId);
        }
    }

    // === Retry wrapper ===
    static async _executeWithRetry(requestFn, retryConfig, method, url, suppressGlobalError = false) {
        let lastError = null;

        for (let attempt = 1; attempt <= retryConfig.maxRetries; attempt++) {
            try {
                const result = await Promise.resolve(requestFn());

                // ADD THIS: If server returned error (4xx/5xx), don't retry
                if (!result.success && result.status >= 400 && result.status < 600) {
                    this._log(`Server error ${result.status}, not retrying: ${url}`);
                    return result; // ← Return error immediately
                }

                if (attempt > 1) this._log(`${method} succeeded on attempt ${attempt}: ${url}`);
                return result;
            } catch (error) {
                lastError = error;

                const errorInfo = this._categorizeError(error);
                const shouldRetry =
                    attempt < retryConfig.maxRetries &&
                    retryConfig.retryableErrors.includes(errorInfo.type);

                if (shouldRetry) {
                    const delay = retryConfig.retryDelay * Math.pow(retryConfig.retryMultiplier, attempt - 1);
                    console.warn(`${method} failed (attempt ${attempt}/${retryConfig.maxRetries}): ${errorInfo.message}. Retrying in ${delay}ms...`);
                    await this._delay(delay);
                } else break;
            }
        }

        const errorResult = this._handleError(lastError, method, url);

        if (!suppressGlobalError && this.globalErrorCallback) {
            try {
                this.globalErrorCallback(errorResult.error.message, errorResult.error);
            } catch (callbackError) {
                console.error('Global error callback failed:', callbackError);
            }
        }

        throw new Error(errorResult.error.message);
    }

    // === Utilities ===
    static _delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    static _categorizeError(error) {
        if (error.name === 'AbortError') return { type: 'TimeoutError', message: 'Request timed out' };
        if (error instanceof TypeError) return { type: 'NetworkError', message: 'Network error - please check your connection' };
        return { type: error.name || 'RequestError', message: error.message || 'Unknown error' };
    }

    static _buildQueryString(params) {
        if (!params || Object.keys(params).length === 0) return '';
        return Object.entries(params)
            .flatMap(([key, val]) => Array.isArray(val)
                ? val.map(v => `${encodeURIComponent(key)}=${encodeURIComponent(v)}`)
                : `${encodeURIComponent(key)}=${encodeURIComponent(val)}`
            ).join('&');
    }

    static async _handleResponse(response, method, url) {
        const contentType = response.headers.get('Content-Type') || '';
        let data = null;
        let errorMessage = null;

        try {
            if (contentType.includes('application/json')) {
                data = await response.json();
            } else if (contentType.includes('text/')) {
                data = await response.text();
            } else {
                data = await response.blob();
            }
        } catch (e) {
            // Parsing failed
        }

        if (response.status === 401 || response.status === 403) {
            const result = {
                success: false,
                unauthorized: true,
                status: response.status,
                error: {
                    message: data?.message || (response.status === 401 ? 'Unauthorized' : 'Forbidden'),
                    status: response.status,
                    data
                }
            };

            // Optionally auto-redirect (client-side fallback)
            if (this.autoRedirectOnAuthFailure) {
                const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                // change path if your Identity login path is different
                window.location.href = `/Identity/Account/Login?ReturnUrl=${returnUrl}`;
                // note: we still return the result in case caller needs it
            }

            return result;
        }

        if (!response.ok) {
            // Instead of throw → return structured error
            errorMessage = data?.message || data || `HTTP ${response.status}`;
            return {
                success: false,
                error: {
                    message: errorMessage,
                    status: response.status,
                    statusText: response.statusText,
                    data
                },
                status: response.status
            };
        }

        return {
            success: true,
            data,
            status: response.status,
            statusText: response.statusText,
            contentType
        };
    }

    //static async _handleResponse(response, method, url) {
    //    const contentType = response.headers.get('Content-Type') || '';

    //    if (response.status === 204) {
    //        return { success: true, data: null, status: 204, statusText: 'No Content' };
    //    }

    //    if (!response.ok) {
    //        let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
    //        try {
    //            if (contentType.includes('application/json')) {
    //                const err = await response.json();
    //                errorMessage = err.message || err.title || errorMessage;
    //            } else if (contentType.includes('text/')) {
    //                const text = await response.text();
    //                errorMessage = text || errorMessage;
    //            }

    //            return {
    //                success: false,
    //                error: {
    //                    message: error,
    //                    status: response.status,
    //                    statusText: response.statusText,
    //                    data
    //                },
    //                status: response.status
    //            };
    //        } catch { /* ignore */ }


            
    //    }

    //    let data;
    //    if (contentType.includes('application/json')) data = await response.json();
    //    else if (contentType.includes('text/')) data = await response.text();
    //    else data = await response.blob();

    //    return {
    //        success: true,
    //        data,
    //        status: response.status,
    //        statusText: response.statusText,
    //        headers: Object.fromEntries(response.headers.entries()),
    //        contentType
    //    };
    //}

    static _handleError(error, method, url) {
        const errType = error.name === 'AbortError'
            ? 'TimeoutError'
            : (error instanceof TypeError ? 'NetworkError' : error.name || 'RequestError');

        const message = error.message || 'An unknown error occurred';

        const result = {
            success: false,
            error: {
                message,
                type: errType,
                method,
                url,
                timestamp: new Date().toISOString()
            }
        };
        console.error(`${method} Error:`, result);
        return result;
    }

    // === File upload helpers ===
    static async uploadMultipleFiles(url, files, additionalData = {}, options = {}) {
        const formData = new FormData();
        Array.from(files).forEach(file => formData.append('files', file));
        Object.entries(additionalData).forEach(([k, v]) =>
            Array.isArray(v) ? v.forEach(item => formData.append(k, item)) : formData.append(k, v)
        );

        const uploadOptions = {
            timeout: 300000,
            retry: { maxRetries: 2, retryDelay: 3000, retryableErrors: ['NetworkError', 'TimeoutError'] },
            ...options,
            isFormData: true
        };

        this._log(`Uploading ${files.length} files to ${url}`);
        return await this.post(url, formData, uploadOptions);
    }

    static async uploadFileInChunks(url, file, options = {}) {
        const chunkSize = options.chunkSize || 1024 * 1024;
        const totalChunks = Math.ceil(file.size / chunkSize);

        this._log(`Uploading "${file.name}" in ${totalChunks} chunks`);

        for (let i = 0; i < totalChunks; i++) {
            const start = i * chunkSize;
            const end = Math.min(start + chunkSize, file.size);
            const chunk = file.slice(start, end);

            const formData = new FormData();
            formData.append('chunk', chunk);
            formData.append('fileName', file.name);
            formData.append('chunkIndex', i.toString());
            formData.append('totalChunks', totalChunks.toString());
            formData.append('fileSize', file.size.toString());

            const result = await this.post(url, formData, {
                timeout: 60000,
                suppressGlobalError: i < totalChunks - 1
            });

            if (!result.success) throw new Error(`Chunk upload failed: ${result.error.message}`);

            if (options.onProgress) {
                const progress = Math.round(((i + 1) / totalChunks) * 100);
                options.onProgress(end, file.size, progress);
            }
        }

        this._log(`File "${file.name}" uploaded successfully`);
        return { success: true, message: 'File uploaded successfully' };
    }

    // === Anti-forgery token ===
    static _getAntiForgeryToken() {
        const meta = document.querySelector('meta[name="__RequestVerificationToken"]');
        if (meta) return meta.getAttribute('content');

        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        if (input) return input.value;

        return null;
    }
}


window.AjaxService = AjaxService;