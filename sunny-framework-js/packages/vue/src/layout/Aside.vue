<template>
  <el-aside :class="{'main-collapse':collapse}">
    <el-scrollbar>
      <slot name="logo"/>
      <el-menu :collapse="collapse" background-color="#304156" text-color="#bfcbd9" active-text-color="#409EFF" :default-active="route.path" router>
        <Menu :collapse="collapse" :menu-tree="props.menus"/>
      </el-menu>
    </el-scrollbar>
  </el-aside>
</template>
<script setup lang="ts">
import {computed} from 'vue'
import Menu from './Menu.vue';
import {useAppStore} from '../store'
import {useRoute} from 'vue-router'
import type {MenuModel} from "./menu.ts";

const appStore = useAppStore()

const props = defineProps<{
  menus: Array<MenuModel>
}>()

const route = useRoute()
const collapse = computed(() => appStore.aside.collapse)

</script>