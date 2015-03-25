//项目中的定时器频率为100khz,将周期设定为20ms（舵机可用）
#include "stm32f10x.h"
extern u8 Is_System_Normal;
        
u16 TIM3_CCR1_VAL;      //TIM3_CH1->一号舵机
u16 TIM3_CCR2_VAL;     //TIM3_CH2->二号舵机
u16 TIM3_CCR3_VAL;      //TIM3_CH3->三号舵机
u16 TIM3_CCR4_VAL;      //TIM3_CH4->四号舵机

u16 TIM4_CCR1_VAL;     //TIM3_CH1->五号舵机
u16 TIM4_CCR2_VAL;     //TIM3_CH2->六号舵机
u16 TIM4_CCR3_VAL;      //TIM3_CH3->七号舵机
u16 TIM4_CCR4_VAL;      //TIM3_CH4->八号舵机


void TIM3_GPIO_Config()
{
	GPIO_InitTypeDef GPIO_InitStructure;
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3,ENABLE);     //	打开定时器外部时钟使能
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA|RCC_APB2Periph_GPIOB,ENABLE);  
	//
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_6|GPIO_Pin_7;    //GPIOA.6->TIM3_CH1,GPIOA.7->TIM3_CH2
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP;        //复用推完输出
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOA,&GPIO_InitStructure);
	
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_0|GPIO_Pin_1;     //GPIOB.0->TIM3_CH3,GPIOB.1->TIM3_CH4
	GPIO_Init(GPIOB,&GPIO_InitStructure);
}
//默认的占空比的初始值要改（复位状态）////////////////////////////////////////////////////////////
void TIM3_Mode_Config()
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_OCInitTypeDef  TIM_OCInitStructure;
	
	TIM_DeInit(TIM3);//缺省值配置
	TIM_TimeBaseStructure.TIM_Period=1999;//设置pwm波的周期20MS
    TIM_TimeBaseStructure.TIM_Prescaler=719;//TIMCLK的频率为100khz
	TIM_TimeBaseStructure.TIM_ClockDivision=TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up;//向上计数模式
	TIM_TimeBaseInit(TIM3,&TIM_TimeBaseStructure);
	//Chnnel1通道的设置
	TIM_OCInitStructure.TIM_OCMode=TIM_OCMode_PWM1;    //TIM_CNT<TIM_CCRx时为高电平，TIM_CNT>TIM_CCRx时为高低平，
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=92;           //设定占空比
	TIM_OCInitStructure.TIM_OCPolarity=TIM_OCPolarity_High;
	TIM_OC1Init(TIM3,&TIM_OCInitStructure);
	
	TIM_OC1PreloadConfig(TIM3,TIM_OCPreload_Enable);   //使能CCR1的预装在寄存器
	//Chnnel2通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=47;
	TIM_OC2Init(TIM3,&TIM_OCInitStructure);
 
	TIM_OC2PreloadConfig(TIM3,TIM_OCPreload_Enable);   //使能CCR2的预装在寄存器
	 //Chnnel3通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=108;
	TIM_OC3Init(TIM3,&TIM_OCInitStructure);
  
	TIM_OC3PreloadConfig(TIM3,TIM_OCPreload_Enable); //使能CCR3的预装在寄存器	
	 //Chnnel4通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=192;
	TIM_OC4Init(TIM3,&TIM_OCInitStructure);
  
	TIM_OC4PreloadConfig(TIM3,TIM_OCPreload_Enable); //使能CCR4的预装在寄存器
		
	TIM_ARRPreloadConfig(TIM3,ENABLE);         //使能TIM3重载寄存器	
	TIM_Cmd(TIM3,ENABLE);                     //打开定时器
	
}



void TIM4_GPIO_Config()
{
	GPIO_InitTypeDef GPIO_InitStructure;
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM4,ENABLE);     //	打开定时器外部时钟使能
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB,ENABLE);  
	//
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_6|GPIO_Pin_7|GPIO_Pin_8|GPIO_Pin_9;    //GPIOB.6->TIM4_CH1,GPIOB.7->TIM4_CH2,GPIOB.8->TIM4_CH3,GPIOB.9->TIM4_CH4
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP;        //复用推完输出
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOB,&GPIO_InitStructure);
}
//////////////////////////////////////初始值///////////////////////////////////////////////////////
void TIM4_Mode_Config()
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_OCInitTypeDef  TIM_OCInitStructure;
	
	TIM_DeInit(TIM4);//缺省值配置
	TIM_TimeBaseStructure.TIM_Period=1999;//设置pwm波的周期20MS
    TIM_TimeBaseStructure.TIM_Prescaler=719;//TIMCLK的频率为100khz
	TIM_TimeBaseStructure.TIM_ClockDivision=TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up;//向上计数模式
	TIM_TimeBaseInit(TIM4,&TIM_TimeBaseStructure);
	//Chnnel1通道的设置
	TIM_OCInitStructure.TIM_OCMode=TIM_OCMode_PWM1;    //TIM_CNT<TIM_CCRx时为高电平，TIM_CNT>TIM_CCRx时为高低平，
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=92;           //设定占空比
	TIM_OCInitStructure.TIM_OCPolarity=TIM_OCPolarity_High;
	TIM_OC1Init(TIM4,&TIM_OCInitStructure);
	
	TIM_OC1PreloadConfig(TIM4,TIM_OCPreload_Enable);   //使能CCR1的预装在寄存器
	//Chnnel2通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=220;
	TIM_OC2Init(TIM4,&TIM_OCInitStructure);
 
	TIM_OC2PreloadConfig(TIM4,TIM_OCPreload_Enable);   //使能CCR2的预装在寄存器
	 //Chnnel3通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=160;
	TIM_OC3Init(TIM4,&TIM_OCInitStructure);
  
	TIM_OC3PreloadConfig(TIM4,TIM_OCPreload_Enable); //使能CCR3的预装在寄存器	
	 //Chnnel4通道的设置
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=50;
	TIM_OC4Init(TIM4,&TIM_OCInitStructure);
  
	TIM_OC4PreloadConfig(TIM4,TIM_OCPreload_Enable); //使能CCR4的预装在寄存器
		
	TIM_ARRPreloadConfig(TIM4,ENABLE);         //使能TIM3重载寄存器	
	TIM_Cmd(TIM4,ENABLE);                     //打开定时器
	

}

//将占空比改变为初始状态（复位值）////////////////////////////////////////////
void Reset_Duoji()
{
    TIM_SetCompare1(TIM3,92);	
    TIM_SetCompare2(TIM3,47);	
    TIM_SetCompare3(TIM3,108);
    TIM_SetCompare4(TIM3,192);	

    TIM_SetCompare1(TIM4,92);	
    TIM_SetCompare2(TIM4,220);	
    TIM_SetCompare3(TIM4,160);
    TIM_SetCompare4(TIM4,50);
	
}

void Control_Duoji(u8 *Control_Duoji)//解包控制舵机函数
{
	//左臂
	 TIM3_CCR1_VAL=Control_Duoji[23];    //手   
	 TIM3_CCR2_VAL=Control_Duoji[18];   //肘子   
	 TIM3_CCR3_VAL=Control_Duoji[15];   //肩部横向     
	 TIM3_CCR4_VAL=Control_Duoji[17];   //肩部纵向      
  //右臂
	 TIM4_CCR1_VAL=Control_Duoji[36];     //手 
	 TIM4_CCR2_VAL=Control_Duoji[31];     //肘子 
	 TIM4_CCR3_VAL=Control_Duoji[28];    //肩部横向   
	 TIM4_CCR4_VAL=Control_Duoji[30];	//肩部纵向 
	
     TIM_SetCompare1(TIM3,TIM3_CCR1_VAL);	
     TIM_SetCompare2(TIM3,TIM3_CCR2_VAL);	
     TIM_SetCompare3(TIM3,TIM3_CCR3_VAL);
     TIM_SetCompare4(TIM3,TIM3_CCR4_VAL);	

     TIM_SetCompare1(TIM4,TIM4_CCR1_VAL);	
     TIM_SetCompare2(TIM4,TIM4_CCR2_VAL);	
     TIM_SetCompare3(TIM4,TIM4_CCR3_VAL);
     TIM_SetCompare4(TIM4,TIM4_CCR4_VAL);
     Is_System_Normal=1;
}

void  TIM3_Init()
{
	TIM3_GPIO_Config();
	TIM3_Mode_Config();
	Reset_Duoji();
}

void  TIM4_Init()
{
	TIM4_GPIO_Config();
	TIM4_Mode_Config();
	Reset_Duoji();
	
}



