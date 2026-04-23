import { useSessionStore } from '@/app/store/sessionStore'

function normalize(binding) {
    const v = binding?.value
    if (!v) return { mode: 'none', keys: [] }
    if (typeof v === 'string') return { mode: 'one', keys: [v] }
    if (Array.isArray(v)) return { mode: 'all', keys: v }
    if (v.any && Array.isArray(v.any)) return { mode: 'any', keys: v.any }
    if (v.all && Array.isArray(v.all)) return { mode: 'all', keys: v.all }
    return { mode: 'none', keys: [] }
}

function check(store, { mode, keys }) {
    if (mode === 'none') return true
    if (mode === 'one') return store.has(keys[0])
    if (mode === 'any') return store.hasAny(keys)
    if (mode === 'all') return store.hasAll(keys)
    return true
}

function hide(el) {
    if (el.__vcanDisplay == null) {
        const cs = getComputedStyle(el)
        el.__vcanDisplay = cs.display && cs.display !== 'none' ? cs.display : 'block'
    }
    el.style.display = 'none'
}

function show(el) {
    el.style.display = el.__vcanDisplay || 'block'
}

function setDisabled(el, isDisabled) {
    if (el.__vcanDisabledOrig == null) el.__vcanDisabledOrig = !!el.disabled
    el.disabled = !!isDisabled
    el.setAttribute('aria-disabled', String(!!isDisabled))
}

function apply(el, binding) {
    const store = useSessionStore()
    const cfg = normalize(binding)
    const ok = check(store, cfg)
    const disable = binding?.modifiers?.disable

    if (disable) {
        setDisabled(el, !ok)
        return
    }

    ok ? show(el) : hide(el)
}

export default {
    install(app) {
        app.directive('can', {
            mounted(el, binding) { apply(el, binding) },
            updated(el, binding) { apply(el, binding) },
            unmounted(el) {
                delete el.__vcanDisplay
                delete el.__vcanDisabledOrig
            },
        })
    },
}
