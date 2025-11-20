<template>
  <template v-for="t in props.menuTree">
    <el-sub-menu v-if="t.children" :data="t" :index="t.name">
      <template #title>
        <el-icon v-if="t.icon">
          <component :is="t.icon"/>
        </el-icon>
        <span>{{ t.name }}</span>
      </template>
      <menu :menuTree="t.children"/>
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

<script lang="ts" setup>
import type {MenuModel} from './menu.ts'

const props = defineProps<{
  collapse: boolean,
  menuTree: Array<MenuModel>
}>()

</script>