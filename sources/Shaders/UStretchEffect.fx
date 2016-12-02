sampler2D input : register(S0);  
float factor    : register(C0);
float alpha     : register(C1);
float  seed     : register(C2);
float  width    : register(C3);

float ease_out(float t){
	t = t - 1.0;
	return (t*t*t*t*t + 1.0);
}

//
//http://neareal.net/index.php?ComputerGraphics%2FHLSL%2FCommon%2FGenerateRandomNoiseInPixelShader
//
float GetRandomNumber(float2 texCoord)
{
    return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) * (int)seed) * 43758.5453);
}

float4 main(float2 uv : TEXCOORD) : COLOR  
{ 
  float limit = clamp(factor, 0, 1.0);
  limit = 1.0 - ease_out(limit);
  //limit = 1.0 - limit;
  
  if(uv.x <=0.5){
	//uv.x= 0.5 - ((0.5 - uv.x)*limit);
	uv.x= 0.5 - ((0.5 - uv.x)*limit);
	//uv.x= GetRandomNumber(uv);
  }else{
	//uv.x = 0.5 + ((uv.x-0.5)*limit);
	uv.x = 0.5 + ((uv.x-0.5)*limit);
  }
  
	float4 color = tex2D( input , uv.xy);
	color.a *= alpha;
    return color;
}
