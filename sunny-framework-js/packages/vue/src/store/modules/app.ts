const app = {
    state: {
        aside: {
            collapse: false
        }
    },
    mutations: {
        TOGGLE_ASIDE: (state: any) => {
            state.aside.collapse = !state.aside.collapse
        }
    },
    actions: {
        TOGGLE_ASIDE_ACTION({commit}: any) {
            commit('TOGGLE_ASIDE')
        }
    }
}
export default app