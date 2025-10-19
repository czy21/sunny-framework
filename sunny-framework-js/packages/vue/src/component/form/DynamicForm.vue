<template>
  <el-form ref="formRef" :model="formData">
    <el-row>
      <el-col :span="option.span" v-for="item in option.list">
        <el-form-item :label="item.name" :prop="item.prop" :rules="item.rules || []">
          <el-input v-if="item.type === 'input'" v-model="formData[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder" clearable></el-input>
          <el-input-number v-else-if="item.type === 'number'" v-model="formData[item.prop]" :disabled="item.disabled" controls-position="right"></el-input-number>
          <el-date-picker v-else-if="item.type === 'date'" type="date" v-model="formData[item.prop]" :value-format="item.format"></el-date-picker>
          <el-select v-else-if="item.type === 'select'" v-model="formData[item.prop]" :disabled="item.disabled" :placeholder="item.placeholder" clearable>
            <el-option v-for="opt in item.options" :label="opt.label" :value="opt.value"></el-option>
          </el-select>
          <slot :name="item.prop" v-else/>
        </el-form-item>
      </el-col>
    </el-row>

    <el-form-item>
      <el-button type="primary" @click="handleSubmit">保存</el-button>
      <el-button @click="handleCancel">取消</el-button>
    </el-form-item>
  </el-form>
</template>

<script lang="ts" setup>
import {DynamicFormOption} from './DynamicFormType.ts';
import {PropType, ref} from 'vue';

const {option,formData} = defineProps({
  option: {
    type: Object as PropType<DynamicFormOption>,
    required: true
  },
  formData: {
    type: Object,
    required: true
  },
});

const emit = defineEmits<{
  'submit': []
  'cancel': []
}>()

const formRef = ref(null);

const handleSubmit = () => {
  emit('submit')
}

const handleCancel = () => {
  emit('cancel');
}

defineExpose({
  formRef
})

</script>