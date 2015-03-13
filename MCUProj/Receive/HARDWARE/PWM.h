#ifndef __PWM_H
#define __PWM_H
#include "stm32f10x.h"

void  TIM3_Init(void);
void  TIM4_Init(void);
void  Reset_Duoji(void);
void  Control_Duoji(u8 *);
#endif
