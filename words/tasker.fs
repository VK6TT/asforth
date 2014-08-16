\ tasker.fs : words for managing tasks

\ the active index into the task list
cvar tidx

\ count register for each task: max 31 tasks
\ is an array of 31 bytes
cvar tcnt
30 allot

( -- n )
\ fetch task index: verifies index is valid
\ adjusts index if count is odd ?
: tidx@
  tidx c@ 
  \ verify index is below 31
  dup 30 >
  if
    \ greater than 30 so 0
    0:
    dup tidx c!
  then
;

( idx -- cnt )
\ get count for a slot
\ idx: index of slot
: task.cnt@
  tcnt + c@
;

\ increment tcnt array element using idx as index
( idx -- )
: task.cnt+
  tcnt + 1+c!
;

( n idx -- )
\ set tcnt array element using idx as index
: task.cnt!
  tcnt + c!
;

\ array of task slots in eeprom : max 31 tasks 62 bytes
\ array is a binary process tree
\                        0                          125 ms
\             1                      2              250 ms
\      3           4           5           6        500 ms
\   7     8     9    10     11   12     13   14     1 s
\ 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30   2 s
edp con tasks
edp 62 + to edp

( -- )
\ increment task index to next task idx
\ assume array flat layout and next idx = idx*2 + 1
: tidx+
  tidx@ 2* 1+ 
  \ if slot count is odd then 1+
  tidx@ task.cnt@
  1 and +
  tidx c!
;

( idx -- task )
\ get a task at idx slot
: task@
  2* tasks + @e 
;

( addr idx -- ) 
\ store a task in a slot
\ idx is the slot index range: 0 to 30
: task!
  2*
  tasks +
  !e
;

( idx -- )
\ clear task at idx slot
\ replaces task with noop
: task.clr 
  ['] noop swap task!
;

( -- )
\ execute active task and step to next task
: tasks.ex
  \ increment count for task slot
  tidx@ task.cnt+
  tidx@ task@ exec
  tidx+
;

\ time in ms since last tasks.ex
var tasks.ms
\ how often in milliseconds to execute a task
\ default to 25 ms
25 val tasks.exms


( -- )
\ execute tasks.ex every 25 ms
: tasks.tick
  ms @ tasks.ms @ - tasks.exms u> if ms @ tasks.ms ! tasks.ex then 
;

( -- )
\ clear all tasks
: tasks.clr
  \ iterate 0 to 30 and clear tcnt[] and set tasks[] to noop
  0
  begin
    0 tidx c!
    0 over task.cnt!
    dup task.clr 
    1+ 
    dup 30 >  
  until
  drop
;

( -- )
\ start tasking
: tasks.run
  \ set taskms to ms
  T0init
  ms @ tasks.ms !
  ['] tasks.tick to pause
;

( -- )
\ reset tasker
\ all tasks are reset to noop
: tasks.reset
  tasks.clr
  tasks.run
;

( -- )
\ stop tasks from running
: tasks.stop
  ['] noop to pause
;
