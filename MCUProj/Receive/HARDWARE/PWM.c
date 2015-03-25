//��Ŀ�еĶ�ʱ��Ƶ��Ϊ100khz,�������趨Ϊ20ms��������ã�
#include "stm32f10x.h"
extern u8 Is_System_Normal;
        
u16 TIM3_CCR1_VAL;      //TIM3_CH1->һ�Ŷ��
u16 TIM3_CCR2_VAL;     //TIM3_CH2->���Ŷ��
u16 TIM3_CCR3_VAL;      //TIM3_CH3->���Ŷ��
u16 TIM3_CCR4_VAL;      //TIM3_CH4->�ĺŶ��

u16 TIM4_CCR1_VAL;     //TIM3_CH1->��Ŷ��
u16 TIM4_CCR2_VAL;     //TIM3_CH2->���Ŷ��
u16 TIM4_CCR3_VAL;      //TIM3_CH3->�ߺŶ��
u16 TIM4_CCR4_VAL;      //TIM3_CH4->�˺Ŷ��


void TIM3_GPIO_Config()
{
	GPIO_InitTypeDef GPIO_InitStructure;
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3,ENABLE);     //	�򿪶�ʱ���ⲿʱ��ʹ��
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA|RCC_APB2Periph_GPIOB,ENABLE);  
	//
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_6|GPIO_Pin_7;    //GPIOA.6->TIM3_CH1,GPIOA.7->TIM3_CH2
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP;        //�����������
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOA,&GPIO_InitStructure);
	
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_0|GPIO_Pin_1;     //GPIOB.0->TIM3_CH3,GPIOB.1->TIM3_CH4
	GPIO_Init(GPIOB,&GPIO_InitStructure);
}
//Ĭ�ϵ�ռ�ձȵĳ�ʼֵҪ�ģ���λ״̬��////////////////////////////////////////////////////////////
void TIM3_Mode_Config()
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_OCInitTypeDef  TIM_OCInitStructure;
	
	TIM_DeInit(TIM3);//ȱʡֵ����
	TIM_TimeBaseStructure.TIM_Period=1999;//����pwm��������20MS
    TIM_TimeBaseStructure.TIM_Prescaler=719;//TIMCLK��Ƶ��Ϊ100khz
	TIM_TimeBaseStructure.TIM_ClockDivision=TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up;//���ϼ���ģʽ
	TIM_TimeBaseInit(TIM3,&TIM_TimeBaseStructure);
	//Chnnel1ͨ��������
	TIM_OCInitStructure.TIM_OCMode=TIM_OCMode_PWM1;    //TIM_CNT<TIM_CCRxʱΪ�ߵ�ƽ��TIM_CNT>TIM_CCRxʱΪ�ߵ�ƽ��
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=92;           //�趨ռ�ձ�
	TIM_OCInitStructure.TIM_OCPolarity=TIM_OCPolarity_High;
	TIM_OC1Init(TIM3,&TIM_OCInitStructure);
	
	TIM_OC1PreloadConfig(TIM3,TIM_OCPreload_Enable);   //ʹ��CCR1��Ԥװ�ڼĴ���
	//Chnnel2ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=47;
	TIM_OC2Init(TIM3,&TIM_OCInitStructure);
 
	TIM_OC2PreloadConfig(TIM3,TIM_OCPreload_Enable);   //ʹ��CCR2��Ԥװ�ڼĴ���
	 //Chnnel3ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=108;
	TIM_OC3Init(TIM3,&TIM_OCInitStructure);
  
	TIM_OC3PreloadConfig(TIM3,TIM_OCPreload_Enable); //ʹ��CCR3��Ԥװ�ڼĴ���	
	 //Chnnel4ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=192;
	TIM_OC4Init(TIM3,&TIM_OCInitStructure);
  
	TIM_OC4PreloadConfig(TIM3,TIM_OCPreload_Enable); //ʹ��CCR4��Ԥװ�ڼĴ���
		
	TIM_ARRPreloadConfig(TIM3,ENABLE);         //ʹ��TIM3���ؼĴ���	
	TIM_Cmd(TIM3,ENABLE);                     //�򿪶�ʱ��
	
}



void TIM4_GPIO_Config()
{
	GPIO_InitTypeDef GPIO_InitStructure;
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM4,ENABLE);     //	�򿪶�ʱ���ⲿʱ��ʹ��
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB,ENABLE);  
	//
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_6|GPIO_Pin_7|GPIO_Pin_8|GPIO_Pin_9;    //GPIOB.6->TIM4_CH1,GPIOB.7->TIM4_CH2,GPIOB.8->TIM4_CH3,GPIOB.9->TIM4_CH4
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP;        //�����������
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOB,&GPIO_InitStructure);
}
//////////////////////////////////////��ʼֵ///////////////////////////////////////////////////////
void TIM4_Mode_Config()
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_OCInitTypeDef  TIM_OCInitStructure;
	
	TIM_DeInit(TIM4);//ȱʡֵ����
	TIM_TimeBaseStructure.TIM_Period=1999;//����pwm��������20MS
    TIM_TimeBaseStructure.TIM_Prescaler=719;//TIMCLK��Ƶ��Ϊ100khz
	TIM_TimeBaseStructure.TIM_ClockDivision=TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up;//���ϼ���ģʽ
	TIM_TimeBaseInit(TIM4,&TIM_TimeBaseStructure);
	//Chnnel1ͨ��������
	TIM_OCInitStructure.TIM_OCMode=TIM_OCMode_PWM1;    //TIM_CNT<TIM_CCRxʱΪ�ߵ�ƽ��TIM_CNT>TIM_CCRxʱΪ�ߵ�ƽ��
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=92;           //�趨ռ�ձ�
	TIM_OCInitStructure.TIM_OCPolarity=TIM_OCPolarity_High;
	TIM_OC1Init(TIM4,&TIM_OCInitStructure);
	
	TIM_OC1PreloadConfig(TIM4,TIM_OCPreload_Enable);   //ʹ��CCR1��Ԥװ�ڼĴ���
	//Chnnel2ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=220;
	TIM_OC2Init(TIM4,&TIM_OCInitStructure);
 
	TIM_OC2PreloadConfig(TIM4,TIM_OCPreload_Enable);   //ʹ��CCR2��Ԥװ�ڼĴ���
	 //Chnnel3ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=160;
	TIM_OC3Init(TIM4,&TIM_OCInitStructure);
  
	TIM_OC3PreloadConfig(TIM4,TIM_OCPreload_Enable); //ʹ��CCR3��Ԥװ�ڼĴ���	
	 //Chnnel4ͨ��������
	TIM_OCInitStructure.TIM_OutputState=TIM_OutputState_Enable;
	TIM_OCInitStructure.TIM_Pulse=50;
	TIM_OC4Init(TIM4,&TIM_OCInitStructure);
  
	TIM_OC4PreloadConfig(TIM4,TIM_OCPreload_Enable); //ʹ��CCR4��Ԥװ�ڼĴ���
		
	TIM_ARRPreloadConfig(TIM4,ENABLE);         //ʹ��TIM3���ؼĴ���	
	TIM_Cmd(TIM4,ENABLE);                     //�򿪶�ʱ��
	

}

//��ռ�ձȸı�Ϊ��ʼ״̬����λֵ��////////////////////////////////////////////
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

void Control_Duoji(u8 *Control_Duoji)//������ƶ������
{
	//���
	 TIM3_CCR1_VAL=Control_Duoji[23];    //��   
	 TIM3_CCR2_VAL=Control_Duoji[18];   //����   
	 TIM3_CCR3_VAL=Control_Duoji[15];   //�粿����     
	 TIM3_CCR4_VAL=Control_Duoji[17];   //�粿����      
  //�ұ�
	 TIM4_CCR1_VAL=Control_Duoji[36];     //�� 
	 TIM4_CCR2_VAL=Control_Duoji[31];     //���� 
	 TIM4_CCR3_VAL=Control_Duoji[28];    //�粿����   
	 TIM4_CCR4_VAL=Control_Duoji[30];	//�粿���� 
	
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



