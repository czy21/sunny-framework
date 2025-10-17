import {Component, createApp as createAppVue} from 'vue'
import App from './App.vue'
import Naive from 'naive-ui'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import VxeUITable from 'vxe-table'
import 'vxe-table/lib/style.css'

const createApp = (rootComponent?: Component) => {
    const app = createAppVue(rootComponent || App)

    app.use(Naive)

    Object.entries(ElementPlusIconsVue).forEach(([k, v]) => app.component(k, v))

    app.use(ElementPlus, {size: 'mini'} as any)
    app.use(VxeUITable)

    return app
}

export {createApp}