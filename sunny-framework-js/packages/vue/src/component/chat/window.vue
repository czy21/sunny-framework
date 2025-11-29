<template>

  <div style="height: 500px">
    <DynamicScroller ref="scroller" :items="props.messages" :min-item-size="50" style="height: 100%">
      <template #default="{ item, index, active }">
        <DynamicScrollerItem :item="item" :active="active" :size-dependencies="[item.data[0].text]" :data-index="index" :data-active="active" style="padding-bottom: 4px">
          <ChatMessage v-if="props.messages[index]" v-model="props.messages[index]" @edit="handleMessageEdit" @delete="handleMessageDelete" :editable="props.editable"/>
        </DynamicScrollerItem>
      </template>
    </DynamicScroller>
  </div>
  <el-divider/>
  <el-input v-model="content" type="textarea" :rows="5" @keydown="handleKeyDown"/>
</template>

<script setup lang="ts">
import {nextTick, ref, watch} from "vue";
import ChatMessage from './message.vue';

const props = defineProps({
  modelValue: String,
  messages: Array,
  editable: Boolean
});

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

const sendMessage = () => {
  const text = content.value.trim();
  if (text.length > 0) {
    if (editingMessage.value) {
      editingMessage.value = null;
      content.value = "";
    } else {
      const messages = [...props.messages];
      messages.push({
        id: messages.length + 1,
        type: "text",
        role: "sender",
        data: [{text}]
      });
      emit("update:messages", messages);
      nextTick(() => {
        scroller.value.scrollToItem(props.messages.length - 1)
        requestAnimationFrame(() => {
          scroller.value.scrollToItem(props.messages.length - 1)
        })
      })
    }
    content.value = ''
  }
}

const handleKeyDown = (event: KeyboardEvent) => {
  console.log(event)
  if (event.key === 'Enter') {
    if (event.shiftKey) {
      return;
    }
    event.preventDefault();
    sendMessage();
  }
};

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
