#include <stm32f10x.h>


void delay_us(u32 num)
{
	u32 i,j;
	for(i=0;i<num;i++)
	{
		for(j=0;j<13;j++);
  }
}

void delay_ms(u32 num)
{
	u32 i,j;
	for(i=0;i<num;i++)
	{
		for(j=0;j<13000;j++);
  }
}

