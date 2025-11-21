<template>
  <el-aside :class="{'main-collapse':collapse}">
    <div class="aside-logo" style="padding: 16px;">
      <slot name="logo"/>
    </div>
    <el-scrollbar style="flex: 1 1 auto; overflow: hidden;">
      <el-menu :collapse="collapse" background-color="#304156" text-color="#bfcbd9" active-text-color="#409EFF" :default-active="route.path" router>
        <Menu :collapse="collapse" :menu-tree="props.menus"/>
      </el-menu>
    </el-scrollbar>
    <div class="aside-footer" style="padding: 12px;text-align: center;color: var(--el-color-white)">
      {{ version.version ? `v${version.version}` : mode }}
    </div>
  </el-aside>
</template>
<script setup lang="ts">
import {computed} from 'vue'
import Menu from './Menu.vue';
import {useAppStore} from '../store'
import {useRoute} from 'vue-router'
import type {MenuModel} from "./menu.ts";

const appStore = useAppStore()

const version = JSON.parse(localStorage.getItem("version")|| "{}")
const mode = import.meta.env.MODE

const props = defineProps<{
  menus: Array<MenuModel>
}>()

const route = useRoute()
const collapse = computed(() => appStore.aside.collapse)

</script>

<style lang="scss" scoped>
.el-aside {
  width: 200px !important;
  background-color: #304156;
  -webkit-transition: width 0.3s ease-in-out;
  transition: width 0.3s ease-in-out;

  display: flex; flex-direction: column;

  .el-menu {
    width: 100%;
    height: 100%;
    border-right: 0;

    .menu-info {
      height: var(--layout-header-height);
    }

    .el-icon-arrow-right:before {
      content: "";
    }
  }
}
.main-collapse {
  width: var(--layout-aside-collapse-width) !important;
}
</style>