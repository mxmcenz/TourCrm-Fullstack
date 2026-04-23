import axios from 'axios'

const api = axios.create({
    baseURL: process.env.VUE_APP_API_BASE || '/api',
    timeout: 5000,
    withCredentials: true,
})

let isRefreshing = false
let failedQueue = []

const processQueue = (error) => {
    failedQueue.forEach(p => error ? p.reject(error) : p.resolve())
    failedQueue = []
}

api.interceptors.response.use(
    r => r,
    async error => {
        const originalRequest = error.config
        const status = error.response?.status

        if (status === 401 && !originalRequest._retry) {
            const url = String(originalRequest.url || '')
            if (/\/auth\/(login|refresh)$/.test(url)) throw error

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject })
                }).then(() => api(originalRequest))
            }

            originalRequest._retry = true
            isRefreshing = true

            try {
                await api.post('/auth/refresh')
                processQueue(null)
                return api(originalRequest)
            } catch (err) {
                processQueue(err)
                throw err
            } finally {
                isRefreshing = false
            }
        }

        throw error
    }
)

export default api
