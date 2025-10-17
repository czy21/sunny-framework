<template>
  <template v-for="t in menuTree">
    <el-sub-menu v-if="t.children" :data="t" :index="t.name">
      <template #title>
        <el-icon v-if="t.icon">
          <component :is="t.icon"/>
        </el-icon>
        <span>{{ t.name }}</span>
      </template>
      <sub-menu :menuTree="t.children"/>
    </el-sub-menu>
    <el-menu-item v-if="!t.children" :data="t" :index="t.path" :route="t.path">
      <el-icon v-if="t.icon">
        <component :is="t.icon"/>
      </el-icon>
      <template #title>
        <span>{{ t.name }}</span>
      </template>
    </el-menu-item>
  </template>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import type {MenuModel} from './menu.ts'

export default defineComponent({
  props: {
    menuTree: {type: Array<MenuModel>},
    collapse: {
      type: Boolean, default() {
        return false
      }
    }
  }
})
</script>