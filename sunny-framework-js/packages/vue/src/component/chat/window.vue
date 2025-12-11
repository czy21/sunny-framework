<template>
  <div :style="{width:'100%',height:props.height}">
    <DynamicScroller ref="scroller" :items="props.messages" :min-item-size="50" style="height: 100%">
      <template #default="{ item, index, active }">
        <DynamicScrollerItem :item="item" :active="active" :size-dependencies="[item.data[0].text]" :data-index="index" :data-active="active" style="padding-bottom: 4px">
          <ChatMessage v-if="props.messages[index]" v-model="props.messages[index]" @edit="handleMessageEdit" @delete="handleMessageDelete" :editable="props.editable"/>
        </DynamicScrollerItem>
      </template>
    </DynamicScroller>
  </div>
  <div>
    <el-upload :on-change="handleUploadImage" action="api/file/upload" :show-file-list="false">
      <el-button type="primary" :icon="Picture"/>
    </el-upload>
  </div>
  <el-input v-model="content" type="textarea" placeholder="输入消息..." :rows="3" @keydown="handleKeyDown"/>
</template>

<script setup lang="ts">
import {nextTick, ref, watch} from "vue";
import ChatMessage from './message.vue';
import {Picture} from '@element-plus/icons-vue'

const props = withDefaults(
    defineProps<{
      modelValue?: string,
      messages?: any[],
      editable: boolean,
      height?: string
    }>(),
    {modelValue: '', messages: [], editable: false, height: "300px"}
)

const emit = defineEmits(["update:messages", "change"]);

const scroller = ref(null)
const content = ref("");
const editingMessage = ref(null)

watch(content, (newVal) => {
  emit("change", newVal);
  if (editingMessage.value) {
    const messages = [...props.messages];
    const index = messages.findIndex(m => m.id === editingMessage.value.id);
    if (index !== -1) {
      messages[index] = {...messages[index], data: [{text: newVal}]};
      emit("update:messages", messages);
    }
  }
});

const addMessage = (data) => {
  const messages = [...props.messages];
  messages.push({
    id: messages.length + 1,
    role: "sender",
    ...data
  });
  emit("update:messages", messages);
  nextTick(() => {
    scroller.value.scrollToItem(props.messages.length - 1)
    requestAnimationFrame(() => {
      scroller.value.scrollToItem(props.messages.length - 1)
    })
  })
}

const sendMessage = () => {
  const text = content.value.trim();
  if (text.length > 0) {
    if (editingMessage.value) {
      editingMessage.value = null;
      content.value = "";
    } else {
      addMessage({type: "text", data: [{text}]})
    }
    content.value = ''
  }
}

const handleKeyDown = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    if (event.shiftKey) {
      return;
    }
    event.preventDefault();
    sendMessage();
  }
};

const handleUploadImage = (file, uploadFiles) => {
  const code = file.response?.code
  if (code === 0) {
    const data = file.response?.data
    addMessage({
      type: "file", data: [
        {
          fileType: data.type,
          fileId: data.id,
          fileUrl: data.fullPath,
          fileName: data.name
        }
      ]
    })
  }
}

const handleMessageEdit = (item) => {
  editingMessage.value = item;
  if (item.type === 'text') {
    content.value = item.data[0].text
  }
}

const handleMessageDelete = (item) => {
  const index = props.messages.findIndex(t => t.id === item.id);
  if (index !== -1) {
    const messages = [...props.messages];
    messages.splice(index, 1);
    emit('update:messages', messages);
  }
}

</script>
