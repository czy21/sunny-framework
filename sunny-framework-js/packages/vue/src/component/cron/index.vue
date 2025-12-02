<template>
  <el-tooltip ref="popperRef" :visible="visible" effect="light" pure trigger="click">

    <template #default>
      <el-input v-model="cronLabel" readonly placeholder="选择定时任务" @click.stop="open" class="cron-picker-input" v-if="props.editable"/>
      <span v-else>
        {{ cronLabel }}
      </span>
    </template>

    <template #content>
      <div class="cron-panel" v-if="props.editable">
        <el-tabs v-model="mode" tab-position="top" class="cron-tabs" @tab-change="handleTabChange">
          <el-tab-pane :label="t.label" :name="t.value" v-for="t in modes" :key="t.value">

            <div v-if="t.value === 'dateTime'" class="cron-tab-content">
              <el-date-picker v-model="dateTime" type="datetime" placeholder="选择日期时间" format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DD HH:mm" :clearable="false" style="width:100%;"/>
            </div>

            <div v-else class="cron-tab-content">
              <div v-if="t.value === 'week'" class="cron-row">
                <label class="cron-label">星期</label>
                <el-select v-model="week" multiple clearable placeholder="选择星期" @clear="onWeekClear" style="flex:1">
                  <el-option v-for="w in weekdays" :key="w.value" :label="w.label" :value="w.value"/>
                </el-select>
              </div>

              <div v-if="t.value === 'month'" class="cron-row">
                <label class="cron-label">日期</label>
                <el-select v-model="month" multiple clearable placeholder="选择日期" @clear="onMonthClear" style="flex:1">
                  <el-option v-for="i in 31" :key="i" :label="i" :value="i"/>
                </el-select>
              </div>

              <div class="cron-row">
                <label class="cron-label">时段</label>
                <el-radio-group v-model="period" size="small" @change="(p)=>time = period0[p]">
                  <el-radio label="midnight">凌晨</el-radio>
                  <el-radio label="morning">上午</el-radio>
                  <el-radio label="afternoon">下午</el-radio>
                  <el-radio label="evening">晚上</el-radio>
                </el-radio-group>
              </div>

              <div class="cron-row">
                <label class="cron-label">时间</label>
                <el-time-select v-model="time" :start="period0[period]" :end="period1[period]" step="00:15" :editable="false" :clearable="false" style="flex:1"/>
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>

        <div class="cron-display-card">
          <el-input v-model="cronValue" readonly style="margin-bottom:6px;"/>
          <ul v-if="nextRuns.length">
            <li v-for="(v,i) in nextRuns" :key="i">{{ v }}</li>
          </ul>
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
import {ref, computed, watch} from 'vue'
import {ElMessage} from 'element-plus'
import later from '@breejs/later'

later.date.localTime()

const props = withDefaults(
    defineProps<{
      modelValue?: string
      editable?: boolean
    }>(),
    {modelValue: '', editable: false}
)

const emit = defineEmits(['update:modelValue', 'change'])

// 显示控制
const visible = ref(false)

// 模式
const modes = ref([
  {
    label: '指定时间',
    value: 'dateTime',
    reset: () => {
      dateTime.value = ''
    },
    validate: () => true
  },
  {
    label: '每天',
    value: 'day',
    reset: () => {
      period.value = 'morning';
      time.value = period0.morning
    },
    validate: () => true
  },
  {
    label: '每周',
    value: 'week',
    reset: () => {
      period.value = 'morning';
      time.value = period0.morning;
      week.value = ['1']
    },
    validate: () => {
      if (week.value.length === 0) {
        ElMessage.warning('每周任务必须至少选择一个星期');
        return false
      }
      return true
    }
  },
  {
    label: '每月',
    value: 'month',
    reset: () => {
      period.value = 'morning';
      time.value = period0.morning;
      month.value = [1]
    },
    validate: () => {
      if (month.value.length === 0) {
        ElMessage.warning('每月任务必须至少选择一个日期');
        return false
      }
      return true
    }
  }
])

// 内部状态
const mode = ref<'day' | 'week' | 'month' | 'dateTime' | ''>('')
const period = ref<'midnight' | 'morning' | 'afternoon' | 'evening' | ''>('')
const time = ref('')
const week = ref<string[]>([])
const month = ref<number[]>([])
const dateTime = ref('')

// 选项
const weekdays = [
  {label: '周一', value: '1'}, {label: '周二', value: '2'}, {label: '周三', value: '3'},
  {label: '周四', value: '4'}, {label: '周五', value: '5'}, {label: '周六', value: '6'}, {label: '周日', value: '0'}
]

const period0: Record<string, string> = {midnight: '00:00', morning: '08:00', afternoon: '12:00', evening: '18:00'}
const period1: Record<string, string> = {midnight: '07:59', morning: '11:59', afternoon: '17:59', evening: '23:59'}

const onWeekClear = () => week.value = ['1']
const onMonthClear = () => month.value = [1]

// ----- display computed -----
const cronLabel = computed(() => {
  if (!mode.value) return ''
  if (mode.value === 'day') return `每天 ${time.value}`
  if (mode.value === 'week') return `每周 ${week.value.join(',')} ${time.value}`
  if (mode.value === 'month') return `每月 ${month.value.join(',')} ${time.value}`
  if (mode.value === 'dateTime' && dateTime.value) {
    const d = new Date(dateTime.value)
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
  }
  return ''
})

const cronValue = computed(() => {
  if (!mode.value) return ''
  if (mode.value !== 'dateTime' && !time.value) return ''

  const [h, m] = time.value.split(':')
  if (mode.value === 'day') return `0 ${m} ${h} * * ?`
  if (mode.value === 'week') {
    const w = week.value.map(w => w === '0' ? '1' : String(Number(w) + 1))
    return `0 ${m} ${h} ? * ${w.join(',')}`
  }
  if (mode.value === 'month') return `0 ${m} ${h} ${month.value.join(',')} * ?`
  if (mode.value === 'dateTime' && dateTime.value) {
    const d = new Date(dateTime.value)
    return `${d.getSeconds()} ${d.getMinutes()} ${d.getHours()} ${d.getDate()} ${d.getMonth() + 1} ?`
  }
  return ''
})

// ----- nextRuns -----
function quartzToLater(cron: string) {
  const parts = cron.trim().split(/\s+/)
  if (parts.length < 6) return null
  const [sec, min, hour, day, month, week] = parts
  const weekMap: Record<string, string> = {'1': 'SUN', '2': 'MON', '3': 'TUE', '4': 'WED', '5': 'THU', '6': 'FRI', '7': 'SAT'}
  const weekExpr = week === '?' ? '' : week.split(',').map(w => weekMap[w] || w).join(',')
  const dayExpr = day === '?' ? '' : day
  return [sec, min, hour, dayExpr || '?', month, weekExpr || '?'].join(' ')
}

const nextRuns = computed(() => {
  if (!cronValue.value) return []
  const laterCron = quartzToLater(cronValue.value)
  if (!laterCron) return []
  try {
    const sched = later.parse.cron(laterCron, true)
    const dates = later.schedule(sched).next(5)
    return dates.map(d => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`)
  } catch {
    return []
  }
})

// ----- methods -----
const open = () => {
  visible.value = true;
  if (!props.modelValue) resetAll()
}
const close = () => visible.value = false
const clear = () => resetAll()
const confirm = () => {
  const validate = modes.value.find(t => t.value === mode.value)?.validate()
  if (!validate) return
  emit('update:modelValue', cronValue.value)
  emit('change', cronValue.value)
  close()
}

const handleTabChange = () => modes.value.forEach(t => t.reset())

// ----- helper -----
function resetAll() {
  modes.value[0].reset();
  mode.value = modes.value[0].value
}

function getPeriodFromTime(t: string) {
  const h = Number(t.split(':')[0])
  if (h < 8) return 'midnight'
  if (h < 12) return 'morning'
  if (h < 18) return 'afternoon'
  return 'evening'
}

// ----- watch modelValue -----
watch(() => props.modelValue, val => {
  if (!val) return resetAll()
  const p = val.split(' ')
  if (p.length < 6) return resetAll()
  const [sec, min, hour, day, mon, weekStr] = p

  if (day === '*' && mon === '*' && weekStr === '?') {
    mode.value = 'day';
    time.value = `${hour}:${min}`;
    period.value = getPeriodFromTime(time.value)
  } else if (day === '?' && mon === '*') {
    mode.value = 'week';
    week.value = weekStr.split(',').map(w => w === '1' ? '0' : String(Number(w) - 1))
    time.value = `${hour}:${min}`;
    period.value = getPeriodFromTime(time.value)
  } else if (mon === '*' && weekStr === '?') {
    mode.value = 'month';
    month.value = day.split(',').map(Number)
    time.value = `${hour}:${min}`;
    period.value = getPeriodFromTime(time.value)
  } else {
    mode.value = 'dateTime'
    dateTime.value = `${new Date().getFullYear()}-${String(mon).padStart(2, '0')}-${String(day).padStart(2, '0')} ${hour}:${min}`
  }
}, {immediate: true})
</script>

<style scoped>
.cron-panel {
  width: 480px;
  padding: 16px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.12);
}

.cron-tab-content {
  padding: 12px 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.cron-row {
  display: flex;
  align-items: center;
  gap: 12px;
}

.cron-label {
  width: 60px;
  text-align: right;
  color: #606266;
  font-size: 14px;
}

.cron-display-card {
  margin-top: 16px;
  padding: 12px;
  background: #f5f5f5;
  border-radius: 6px;
}

.cron-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
  gap: 10px;
}

.cron-picker-input {
  cursor: pointer;
}
</style>