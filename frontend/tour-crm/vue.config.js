const target = process.env.VUE_APP_API_TARGET || 'http://localhost:8081'

module.exports = {
    transpileDependencies: true,
    devServer: {
        port: 5173,
        proxy: {
            '^/api': {target, changeOrigin: true, secure: false},
        },
    },
    pluginOptions: {
        vuetify: {},
    },
}