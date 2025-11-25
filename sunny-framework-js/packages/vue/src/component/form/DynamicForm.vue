<template>
  <el-form ref="formRef" label-width="auto" :model="formData" :label-position="option.labelPosition">
    <el-row>
      <el-col :span="option.span" v-for="item in formItems">
        <el-form-item :label="item.name" :prop="item.prop" :rules="item.rules || []" v-if="item.type === 'inputs'">
          <el-input v-for="(v, i) in formData[item.prop]" :key="i" v-model="formData[item.prop][i]" style="margin-bottom: 5px;">
            <template #append>
              <el-button @click="delInput(item.prop, i)" :disabled="formData[item.prop].length <= 1" type="danger">
                <el-icon>
                  <Delete/>
                </el-icon>
              </el-button>
            </template>
          </el-input>
          <el-button @click="addInput(item.prop)" type="">
            <el-icon>
              <Plus/>
            </el-icon>
          </el-button>
        </el-form-item>
        <el-form-item :label="item.name" :prop="item.prop" :rules="item.rules || []" v-else>
          <el-input v-if="item.type === 'input'" v-model="formData[item.prop]" :disabled="getDisabled(item)" :placeholder="item.placeholder" clearable/>
          <el-input v-else-if="item.type === 'password'" type="password" v-model="formData[item.prop]" :disabled="getDisabled(item)" :placeholder="item.placeholder" clearable/>
          <el-input-number v-else-if="item.type === 'number'" :min="item.min" :max="item.max" v-model="formData[item.prop]" :disabled="getDisabled(item)" controls-position="right" style="width:100%"/>
          <el-date-picker v-else-if="item.type === 'date'" type="date" v-model="formData[item.prop]" :value-format="item.format"/>
          <el-select v-else-if="item.type === 'select'" v-model="formData[item.prop]" :disabled="getDisabled(item)" :placeholder="item.placeholder" clearable>
            <el-option v-for="opt in item.options" :label="opt.label" :value="opt.value"></el-option>
          </el-select>
          <el-radio-group v-else-if="item.type === 'radio'" v-model="formData[item.prop]" :disabled="getDisabled(item)">
            <el-radio v-for="opt in item.options" :value="opt.value">{{ opt.label ?? opt.value }}</el-radio>
          </el-radio-group>
          <el-tag v-else-if="item.type === 'tag'" v-for="t in formData[item.prop]" :key="t[item.options['value']]" closable @close="(e:any)=>handleTagClose(e,item,t)">
            {{ getTagLabel(item, t) }}
          </el-tag>
          <Cron v-else-if="item.type === 'cron'" v-model="formData[item.prop]" :disabled="getDisabled(item)"/>
          <slot :name="item.prop" v-else/>
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item>
      <el-button type="primary" @click="handleSubmit">{{ option.submitText ?? '保存' }}</el-button>
      <el-button @click="handleCancel">{{ option.cancelText ?? '取消' }}</el-button>
    </el-form-item>
  </el-form>
</template>

<script lang="ts" setup>
import {DynamicFormOption} from './DynamicFormType.ts';
import {computed, ref, watch} from 'vue';
import type {FormInstance} from 'element-plus'
import Cron from '../cron/index.vue'

const props = defineProps<{
  option: DynamicFormOption;
  formData: Record<string, any>;
}>();

const option = computed(() => ({
  labelPosition: 'right',
  span: 12,
  list: [],
  ...props.option,
}));

const formData: Record<string, any> = computed(() => props.formData);

const formItems = ref(null)

function addInput(prop: string) {
  if (!Array.isArray(formData.value[prop])) formData.value[prop] = [];
  formData.value[prop].push('');
}

function delInput(prop: string, index: number) {
  if (formData.value[prop].length > 1) {
    formData.value[prop].splice(index, 1);
  }
}

function getDisabled(item) {
  return typeof item.disabled === 'function' ? item.disabled() : item.disabled
}

watch(
    [() => formData],
    () => {
      formItems.value = props.option.items.filter((t: any) => {
        if (typeof t.show === 'function') {
          return t.show(t, formData.value)
        }
        return t.show ?? true
      })
    },
    {immediate: true, deep: true}
)

const emit = defineEmits<{
  'submit': []
  'cancel': []
}>()

const formRef = ref<FormInstance>()

const handleSubmit = () => {
  emit('submit')
}

const handleCancel = () => {
  emit('cancel');
}

const getTagLabel = (item, val) => {
  const label = item.options['label']
  return typeof label === 'function' ? label(val) : val[label]
}

const handleTagClose = (e, item, val) => {
  formData.value[item.prop] = formData.value[item.prop].filter(t => t[item.options['value']] !== val[item.options['value']])
}

defineExpose({
  formRef
})

</script>