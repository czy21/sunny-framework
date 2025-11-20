import { defineStore } from "pinia";

export const useTabStore = defineStore('tab', {
    state: () => ({
        visites: [] as Array<any>,
        maxTabs: 10,
    }),

    actions: {
        persist() {
            localStorage.setItem(
                'user_tabs',
                JSON.stringify(this.visites.map(v => ({ path: v.path, query: v.query })))
            );
        },

        restoreFromCache(router) {
            const raw = localStorage.getItem('user_tabs');
            if (!raw) return;

            const saved = JSON.parse(raw);
            const restored: any[] = [];

            saved.forEach((item: any) => {
                const r = router.resolve({ path: item.path, query: item.query });
                if (r && r.name) {
                    restored.push({
                        ...r,
                        query: r.query,
                        title: r.query.title || r.meta?.title || 'no-name',
                    });
                }
            });

            this.visites = restored;
        },

        addItem(route: any) {
            // 判断是否已存在同 path+query
            const exists = this.visites.some(
                v => v.path === route.path && JSON.stringify(v.query || {}) === JSON.stringify(route.query || {})
            );
            if (exists) return;

            this.visites.push({
                ...route,
                title: route.query?.title || route.meta?.title || 'no-name',
            });

            // 超过最大 tab 数时，删除最早的非 affix tab
            while (this.visites.length > this.maxTabs) {
                const index = this.visites.findIndex(v => !v.meta?.affix);
                if (index >= 0) {
                    this.visites.splice(index, 1);
                } else {
                    // 全部是 affix，强制保留最大数量
                    break;
                }
            }

            this.persist();
        },

        delItem(item: any) {
            this.visites = this.visites.filter(
                v => !(v.path === item.path && JSON.stringify(v.query || {}) === JSON.stringify(item.query || {}))
            );
            this.persist();
        },

        delOtherItems(item: any) {
            this.visites = this.visites.filter(
                v => v.meta?.affix || (v.path === item.path && JSON.stringify(v.query || {}) === JSON.stringify(item.query || {}))
            );
            this.persist();
        },

        delRightItems(item: any) {
            const index = this.visites.findIndex(
                v => v.path === item.path && JSON.stringify(v.query || {}) === JSON.stringify(item.query || {})
            );
            this.visites = this.visites.filter((v, idx) => idx <= index || v.meta?.affix);
            this.persist();
        },

        delLeftItems(item: any) {
            const index = this.visites.findIndex(
                v => v.path === item.path && JSON.stringify(v.query || {}) === JSON.stringify(item.query || {})
            );
            this.visites = this.visites.filter((v, idx) => idx >= index || v.meta?.affix);
            this.persist();
        },

        delAllItems() {
            this.visites = this.visites.filter(v => v.meta?.affix);
            this.persist();
        },
    },
});
