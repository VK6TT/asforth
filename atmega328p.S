.section .progmem
.include "macros.S"
.include "device.S"

.equ TIBSIZE, 0x50   ; 80 characters is one line...


; cpu clock in hertz
.equ F_CPU, 16000000
; baud rate of terminal
.equ BAUD, 38400
.include "drivers/usart_0.S"

.Set rstackstart, RAMEND
.Set stackstart, RAMEND - 50

.include "asforth.S"
