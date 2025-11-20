<template>
  <template v-for="t in props.menuTree">
    <el-sub-menu v-if="t.children" :data="t" :index="t.name">
      <template #title>
        <el-icon v-if="t.icon">
          <component :is="t.icon"/>
        </el-icon>
        <span>{{ t.name }}</span>
      </template>
      <Menu :menuTree="t.children" :collapse="props.collapse"/>
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

defineOptions({
  name: 'Menu'
})


const props = defineProps<{
  collapse: boolean,
  menuTree: Array<MenuModel>
}>()

</script>