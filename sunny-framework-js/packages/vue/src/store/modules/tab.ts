import {defineStore} from "pinia";

export const useTabStore = defineStore('tab', {
    state: () => ({
        // 页面中的完整 tab 列表（包含 title/meta/path 等）
        visites: [],
    }),

    actions: {
        /** 只把 path 存到 localStorage */
        persist() {
            localStorage.setItem('user_tabs', JSON.stringify(this.visites.map(v => v.path)))
        },

        /** 从 localStorage 恢复，仅用 path 还原完整 route 对象 */
        restoreFromCache(router) {
            const raw = localStorage.getItem('user_tabs')
            if (!raw) return

            const paths = JSON.parse(raw)
            const restored: any[] = []

            paths.forEach(path => {
                const r = router.resolve(path)
                if (r && r.name) {
                    restored.push({
                        ...r,
                        title: r.meta?.title || 'no-name',
                    })
                }
            })

            this.visites = restored
        },

        /** 添加标签 */
        addItem(route) {
            if (this.visites.some(v => v.path === route.path)) return

            this.visites.push({
                ...route,
                title: route.meta?.title || 'no-name',
            })
            this.persist()
        },

        /** 删除单个 */
        delItem(item) {
            this.visites = this.visites.filter(v => v.path !== item.path)
            this.persist()
        },

        /** 关闭其他 */
        delOtherItems(item) {
            this.visites = this.visites.filter(v => v.meta?.affix || v.path === item.path)
            this.persist()
        },

        /** 关闭右侧 */
        delRightItems(item) {
            const index = this.visites.findIndex(v => v.path === item.path)
            this.visites = this.visites.filter((v, idx) => idx <= index || v.meta?.affix)
            this.persist()
        },

        /** 关闭左侧 */
        delLeftItems(item) {
            const index = this.visites.findIndex(v => v.path === item.path)
            this.visites = this.visites.filter((v, idx) => idx >= index || v.meta?.affix)
            this.persist()
        },
        /** 关闭全部 */
        delAllItems() {
            this.visites = this.visites.filter(v => v.meta?.affix)
            this.persist()
        },
    },
})
