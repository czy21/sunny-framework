import {defineStore} from 'pinia'

export const useAppStore = defineStore('app', {
    state: () => ({
        aside: {
            collapse: false
        }
    }),
    actions: {
        changeAsideCollapse() {
            this.aside.collapse = !this.aside.collapse
        }
    },
})