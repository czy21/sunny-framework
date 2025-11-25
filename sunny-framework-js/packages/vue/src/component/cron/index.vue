<template>
  <el-tooltip ref="popperRef" :visible="visible" effect="light" pure trigger="click"
              role="dialog" teleported class="cron-picker-popper"
              placement="bottom-start" :hide-after="0" persistent>
    <template #default>
      <el-input ref="inputRef" v-model="display" readonly placeholder="选择定时任务" class="cron-picker-input" @click.stop="open"/>
    </template>

    <template #content>
      <div class="cron-panel" @mousedown.stop>

        <el-tabs v-model="mode" tab-position="top" class="cron-tabs">
          <el-tab-pane label="每天" name="day">
            <div class="cron-tab-content">
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时段</el-col>
                <el-col :span="18">
                  <el-radio-group v-model="period" size="small">
                    <el-radio label="midnight">凌晨</el-radio>
                    <el-radio label="morning">上午</el-radio>
                    <el-radio label="afternoon">下午</el-radio>
                    <el-radio label="evening">晚上</el-radio>
                  </el-radio-group>
                </el-col>
              </el-row>
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时间</el-col>
                <el-col :span="18">
                  <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                  step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
                </el-col>
              </el-row>
            </div>
          </el-tab-pane>

          <el-tab-pane label="每周" name="week">
            <div class="cron-tab-content">
              <el-row class="cron-row" align="middle">
                <el-col :span="6">星期</el-col>
                <el-col :span="18">
                  <el-select v-model="weekdaysSelected" multiple clearable
                             placeholder="选择星期" @clear="onWeekClear" style="width:100%;">
                    <el-option v-for="w in weekdays" :key="w.value" :label="w.label" :value="w.value"/>
                  </el-select>
                </el-col>
              </el-row>
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时段</el-col>
                <el-col :span="18">
                  <el-radio-group v-model="period" size="small">
                    <el-radio label="midnight">凌晨</el-radio>
                    <el-radio label="morning">上午</el-radio>
                    <el-radio label="afternoon">下午</el-radio>
                    <el-radio label="evening">晚上</el-radio>
                  </el-radio-group>
                </el-col>
              </el-row>
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时间</el-col>
                <el-col :span="18">
                  <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                  step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
                </el-col>
              </el-row>
            </div>
          </el-tab-pane>

          <el-tab-pane label="每月" name="month">
            <div class="cron-tab-content">
              <el-row class="cron-row" align="middle">
                <el-col :span="6">日期</el-col>
                <el-col :span="18">
                  <el-select v-model="monthDaysSelected" multiple clearable
                             placeholder="选择日期" @clear="onMonthClear" style="width:100%;">
                    <el-option v-for="i in 31" :key="i" :label="i" :value="i"/>
                  </el-select>
                </el-col>
              </el-row>
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时段</el-col>
                <el-col :span="18">
                  <el-radio-group v-model="period" size="small">
                    <el-radio label="midnight">凌晨</el-radio>
                    <el-radio label="morning">上午</el-radio>
                    <el-radio label="afternoon">下午</el-radio>
                    <el-radio label="evening">晚上</el-radio>
                  </el-radio-group>
                </el-col>
              </el-row>
              <el-row class="cron-row" align="middle">
                <el-col :span="6">时间</el-col>
                <el-col :span="18">
                  <el-time-select v-model="time" :start="periodStart[period]" :end="periodEnd[period]"
                                  step="00:15" :editable="false" :clearable="false" style="width:100%;"/>
                </el-col>
              </el-row>
            </div>
          </el-tab-pane>

          <el-tab-pane label="指定时间" name="time">
            <div class="cron-tab-content">
              <el-row class="cron-row" align="middle">
                <el-col :span="6">日期时间</el-col>
                <el-col :span="18">
                  <el-date-picker v-model="dateTime" type="datetime" placeholder="选择日期时间"
                                  format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DD HH:mm" style="width:100%;"/>
                </el-col>
              </el-row>
            </div>
          </el-tab-pane>
        </el-tabs>

        <div class="cron-display-card">
          <div class="cron-display-row">
            <div class="cron-label">Cron</div>
            <el-input v-model="cronDisplay" readonly style="flex:1;"/>
          </div>
          <div v-if="nextRuns.length" class="cron-next-runs">
            <div class="cron-label">最近8次运行</div>
            <ul>
              <li v-for="(v,i) in nextRuns" :key="i">{{ v }}</li>
            </ul>
          </div>
        </div>

        <div class="cron-footer">
          <el-button size="small" @click="clear">清除</el-button>
          <el-button size="small" @click="close">取消</el-button>
          <el-button size="small" type="primary" @click="confirm">确定</el-button>
        </div>
      </div>
    </template>
  </el-tooltip>
</template>

<script setup lang="ts">
import {computed, ref, watch} from 'vue'
import {ElMessage} from 'element-plus'
import cronParser from 'cron-parser'

const props = defineProps({
  modelValue: {type: String, default: ''}
})

const emit = defineEmits(['update:modelValue', 'change'])

const visible = ref(false)
const mode = ref<'day' | 'week' | 'month' | 'time'>('day')

// 时段起始和结束时间
const periodStart = {midnight: '00:00', morning: '08:00', afternoon: '13:00', evening: '18:00'}
const periodEnd = {midnight: '07:59', morning: '11:59', afternoon: '17:59', evening: '23:59'}

const period = ref<'midnight' | 'morning' | 'afternoon' | 'evening'>('morning')
const time = ref(periodStart[period.value]) // 默认时间对应默认时段

const weekdays = [
  {label: '周一', value: '1'},
  {label: '周二', value: '2'},
  {label: '周三', value: '3'},
  {label: '周四', value: '4'},
  {label: '周五', value: '5'},
  {label: '周六', value: '6'},
  {label: '周日', value: '0'},
]

const weekdaysSelected = ref<string[]>(['1'])
const monthDaysSelected = ref<number[]>([1])
const dateTime = ref('')

// 清空逻辑
const onWeekClear = () => (weekdaysSelected.value = ['1'])
const onMonthClear = () => (monthDaysSelected.value = [1])

// 切换模式时自动补值
watch(mode, m => {
  period.value = 'morning'
  time.value = periodStart[period.value]

  if (m !== 'week') weekdaysSelected.value = []
  if (m !== 'month') monthDaysSelected.value = []
  if (m !== 'time') dateTime.value = ''

  if (m === 'week' && weekdaysSelected.value.length === 0)
    weekdaysSelected.value = ['1']

  if (m === 'month' && monthDaysSelected.value.length === 0)
    monthDaysSelected.value = [1]
})

// 切换时段时，自动把时间改为该时段起始时间
watch(period, p => {
  time.value = periodStart[p]
})

// 展示文字
const display = computed(() => {
  if (mode.value === 'day') return `每天 ${time.value}`
  if (mode.value === 'week') return `每周 ${weekdaysSelected.value.join(',')} ${time.value}`
  if (mode.value === 'month') return `每月 ${monthDaysSelected.value.join(',')} ${time.value}`

  // 指定时间模式，显示完整日期时间
  if (mode.value === 'time' && dateTime.value) {
    const dt = new Date(dateTime.value)
    const yyyy = dt.getFullYear()
    const mm = String(dt.getMonth() + 1).padStart(2, '0')
    const dd = String(dt.getDate()).padStart(2, '0')
    const hh = String(dt.getHours()).padStart(2, '0')
    const min = String(dt.getMinutes()).padStart(2, '0')
    const ss = String(dt.getSeconds()).padStart(2, '0')
    return `${yyyy}-${mm}-${dd} ${hh}:${min}:${ss}`
  }
  return ''
})

// 生成 cron 表达式
const cronDisplay = computed(() => {
  if (mode.value === 'day') {
    const [h, m] = time.value.split(':')
    return `${m} ${h} * * *`
  }
  if (mode.value === 'week') {
    const [h, m] = time.value.split(':')
    return `${m} ${h} * * ${weekdaysSelected.value.join(',')}`
  }
  if (mode.value === 'month') {
    const [h, m] = time.value.split(':')
    return `${m} ${h} ${monthDaysSelected.value.join(',')} * *`
  }
  if (mode.value === 'time' && dateTime.value) {
    const dt = new Date(dateTime.value)
    return `${dt.getMinutes()} ${dt.getHours()} ${dt.getDate()} ${dt.getMonth() + 1} *`
  }
  return ''
})

// 下 8 次执行
const nextRuns = computed(() => {
  if (!cronDisplay.value) return []
  try {
    const it = cronParser.parse(cronDisplay.value)
    const arr: string[] = []
    for (let i = 0; i < 8; i++) {
      const d = it.next()
      const yyyy = d.getFullYear()
      const mm = String(d.getMonth() + 1).padStart(2, '0')
      const dd = String(d.getDate()).padStart(2, '0')
      const hh = String(d.getHours()).padStart(2, '0')
      const min = String(d.getMinutes()).padStart(2, '0')
      const ss = String(d.getSeconds()).padStart(2, '0')
      arr.push(`${yyyy}-${mm}-${dd} ${hh}:${min}:${ss}`)
    }
    return arr
  } catch {
    return []
  }
})

const open = () => (visible.value = true)
const close = () => (visible.value = false)

const clear = () => {
  period.value = 'morning'
  time.value = periodStart[period.value]
  weekdaysSelected.value = ['1']
  monthDaysSelected.value = [1]
  dateTime.value = ''
}

// 点击确定校验 + 保存
const confirm = () => {
  if (mode.value === 'week' && weekdaysSelected.value.length === 0) {
    return ElMessage.warning('每周任务必须至少选择一个星期')
  }
  if (mode.value === 'month' && monthDaysSelected.value.length === 0) {
    return ElMessage.warning('每月任务必须至少选择一个日期')
  }
  emit('update:modelValue', cronDisplay.value)
  emit('change', cronDisplay.value)
  close()
}

// 初始化 modelValue
watch(() => props.modelValue, val => {
  if (!val) return
  const p = val.split(' ')
  if (p.length < 5) return

  const [m, h, d, mon, w] = p

  if (d === '*' && mon === '*' && w === '*') {
    mode.value = 'day'
    time.value = `${h}:${m}`
  } else if (d === '*' && mon === '*') {
    mode.value = 'week'
    weekdaysSelected.value = w.split(',')
    time.value = `${h}:${m}`
  } else if (mon === '*') {
    mode.value = 'month'
    monthDaysSelected.value = d.split(',').map(Number)
    time.value = `${h}:${m}`
  } else {
    mode.value = 'time'
    const year = new Date().getFullYear()
    dateTime.value = `${year}-${String(mon).padStart(2, '0')}-${String(d).padStart(2, '0')} ${h}:${m}`
  }
}, {immediate: true})
</script>

<style scoped>
.cron-panel {
  width: 500px;
  padding: 12px;
  background-color: #fff;
  border-radius: 6px;
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.cron-tabs >>> .el-tabs__header {
  margin-bottom: 12px;
}

.cron-tab-content {
  padding: 8px 0;
}

.cron-row {
  margin-bottom: 8px;
}

.cron-display-card {
  margin-top: 12px;
  padding: 10px;
  background-color: #f9f9f9;
  border-radius: 4px;
}

.cron-display-row {
  display: flex;
  align-items: center;
  margin-bottom: 6px;
}

.cron-label {
  width: 80px;
  font-weight: 500;
}

.cron-next-runs ul {
  list-style: none;
  padding-left: 0;
  margin: 0;
  max-height: 120px;
  overflow-y: auto;
}

.cron-next-runs li {
  line-height: 24px;
  font-size: 13px;
  color: #555;
}

.cron-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
  gap: 8px;
}
</style>