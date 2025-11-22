import {Component, createApp as createAppVue} from 'vue'
import App from './App.vue'
import { createPinia } from 'pinia'
import Naive from 'naive-ui'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import VxeUIBase from 'vxe-pc-ui'
import 'vxe-pc-ui/es/style.css'

import VxeUITable from 'vxe-table'
import 'vxe-table/lib/style.css'

import './static/var.css'

const createApp = (rootComponent?: Component) => {
    const app = createAppVue(rootComponent || App)
    const pinia = createPinia()

    app.use(pinia)
    app.use(Naive)

    Object.entries(ElementPlusIconsVue).forEach(([k, v]) => app.component(k, v))

    app.use(ElementPlus, {size: 'default'} as any)
    app.use(VxeUIBase).use(VxeUITable)
    return app
}

export {createApp}