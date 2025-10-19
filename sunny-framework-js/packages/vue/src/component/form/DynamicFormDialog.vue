<template>
  <el-dialog :model-value="props.dialogShow" :title="props.dialogTitle" @close="handleCancel">
    <DynamicForm :form-key="props.formKey" ref="editRef" :form-data="props.formData" :option="props.formOption" @submit="handleSubmit" @cancel="handleCancel"/>
  </el-dialog>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import {DynamicForm, DynamicFormOption} from "@sunny-framework-js/vue";
import type {FormInstance} from 'element-plus'

const editRef = ref();

const props = defineProps<{
  dialogShow: boolean,
  dialogTitle: string,
  formKey?: string,
  formOption: DynamicFormOption,
  formData: Record<string, any>
}>()

const emit = defineEmits<{
  'update:dialogShow': [value: boolean]
  'submit': [formEl: FormInstance]
  'cancel': [formEl: FormInstance]
}>()

const handleSubmit = () => {
  emit('submit', editRef.value.formRef)
}

const handleCancel = () => {
  emit('cancel', editRef.value.formRef)
}

</script>