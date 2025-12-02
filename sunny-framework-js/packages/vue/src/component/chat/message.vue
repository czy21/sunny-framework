<template>
  <div :class="['bubble-wrapper', role]">
    <el-popover pure trigger="hover" :placement="role === 'sender' ? 'left-start' : 'right-start'" :popper-style="{ minWidth: '0px', width: 'auto' }">
      <template #default>
        <el-button-group direction="horizontal" size="small" v-if="editable">
          <el-button plain :icon="IconEdit" @click="handleEdit"/>
          <el-button plain :icon="IconDelete" @click="handleDelete"/>
        </el-button-group>
      </template>
      <template #reference>
        <div class="bubble">
          <div v-if="props.modelValue.type === 'text'">
            {{ props.modelValue.data[0].text }}
          </div>
        </div>
      </template>
    </el-popover>
  </div>
</template>

<script setup lang="ts">
import {computed} from "vue"
import {Delete as IconDelete, Edit as IconEdit} from '@element-plus/icons-vue'

const props = defineProps({
  modelValue: {type: Object, required: true},
  editable: Boolean
})

const emit = defineEmits(['edit', 'delete'])

const role = computed(() => (props.modelValue.role ?? 'sender') === 'sender' ? 'sender' : 'receiver')

const handleEdit = () => emit('edit', props.modelValue)
const handleDelete = () => emit('delete', props.modelValue)

</script>

<style scoped>
.bubble-wrapper {
  display: flex;
  margin: 4px 0;
}

.bubble-wrapper.sender {
  justify-content: flex-end;
}

.bubble-wrapper.receiver {
  justify-content: flex-start;
}

.bubble {
  max-width: 60%;
  padding: 8px 12px;
  border-radius: 16px;
  background-color: #f0f0f0;
  cursor: pointer;
  white-space: pre-line;
  word-break: break-word;
}

.bubble-wrapper.sender .bubble {
  background-color: #cce5ff;
}

</style>
