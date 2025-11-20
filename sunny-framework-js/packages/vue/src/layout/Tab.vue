<template>
  <div class="tab-container">
    <div style="display: flex;gap: 8px;height: 100%;align-items: center;">
      <el-tag v-for="t in visites"
              :style="{cursor: 'pointer'}"
              :key="t.path"
              closable
              :type="isActive(t)?'primary':'info'"
              hit
              @contextmenu.prevent="(event) => handleContextMenu(t, event)"
              @close="closeSelected(t)"
              @click="router.push(t.fullPath)">
        {{ t.meta.title }}
      </el-tag>
    </div>

    <!-- 右键菜单 -->
    <el-dropdown ref="contextMenuRef"
                 @command="handleContextMenuCommand"
                 :style="{
                    position: 'fixed',
                    left: contextMenu.x + 'px',
                    top: contextMenu.y + 'px',
                    display: 'block'
                 }">
      <span></span>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item command="close">关闭</el-dropdown-item>
          <el-dropdown-item command="closeOther">关闭其他</el-dropdown-item>
          <el-dropdown-item command="closeLeft">关闭左侧</el-dropdown-item>
          <el-dropdown-item command="closeRight">关闭右侧</el-dropdown-item>
          <el-dropdown-item command="closeAll">关闭所有</el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script lang="ts" setup>
import {ref, watch, reactive, computed, onMounted, nextTick} from "vue";
import {useRouter, useRoute} from "vue-router";
import {useTabStore} from '../store'

const router = useRouter()
const route = useRoute()
const tabStore = useTabStore()

const visites = computed(() => tabStore.visites)
const selectedTab = ref()

/** 右键菜单相关 */
const contextMenuRef = ref()
const contextMenu = reactive({ x: 0, y: 0 })

/** 鼠标右键 */
function handleContextMenu(item, event) {
  contextMenu.x = event.clientX
  contextMenu.y = event.clientY
  selectedTab.value = item

  nextTick(() => contextMenuRef.value.handleOpen())
}

/** 右键菜单命令 */
function handleContextMenuCommand(command) {
  switch (command) {
    case 'close':
      closeSelected(selectedTab.value)
      break
    case 'closeOther':
      closeOther(selectedTab.value)
      break
    case 'closeLeft':
      closeLeft(selectedTab.value)
      break
    case 'closeRight':
      closeRight(selectedTab.value)
      break
    case 'closeAll':
      closeAll()
      break
  }
}

/** 路由变化时：添加标签 / 切换 */
watch(route, () => {
  addTab()
  switchToCurrentTab()
})

/** 初始化：恢复本地缓存 + affix */
onMounted(() => {
  tabStore.restoreFromCache(router)
  initAffixTabs()
  addTab()
})

/** 判断是否激活 */
function isActive(item) {
  return item.path === route.path
}

/** 初始化 affix 标签 */
function initAffixTabs() {
  router.getRoutes().forEach(r => {
    if (r.meta?.affix) {
      tabStore.addItem(r)
    }
  })
}

/** 添加当前路由为 tab */
function addTab() {
  if (route.name && route.meta?.title) {
    tabStore.addItem(route)
  }
}

/** 更新 fullPath */
function switchToCurrentTab() {
  nextTick(() => {
    visites.value.forEach(v => {
      if (v.path === route.path && v.fullPath !== route.fullPath) {
        v.fullPath = route.fullPath
      }
    })
  })
}

/** 关闭当前 */
function closeSelected(item) {
  const index = visites.value.findIndex(v => v.path === item.path)

  const right = visites.value[index + 1]
  const left  = visites.value[index - 1]

  tabStore.delItem(item)

  if (!isActive(item)) return

  if (right) return router.push(right.fullPath)
  if (left) return router.push(left.fullPath)

  router.push('/')
}

/** 关闭右侧 */
function closeRight(item) {
  const index = visites.value.findIndex(v => v.path === item.path)

  tabStore.delRightItems(item)

  if (isActive(item)) {
    const now = visites.value[index]
    if (now) router.push(now.fullPath)
    else toLast()
  }
}

/** 关闭左侧 */
function closeLeft(item) {

  tabStore.delLeftItems(item)

  if (isActive(item)) router.push(item.fullPath)
}

/** 关闭其他 */
function closeOther(item) {
  tabStore.delOtherItems(item)
  router.push(item.fullPath)
}

/** 关闭所有 */
function closeAll() {
  tabStore.delAllItems()
  toLast()
}

/** 跳转到最后一个 */
function toLast() {
  const last = visites.value.at(-1)
  if (last) router.push(last.fullPath)
  else router.push('/')
}
</script>

<style lang="scss" scoped>
.tab-container {
  height: 34px;
  padding: 0 15px;
  border-top: 1px solid var(--el-border-color-light);
}
</style>