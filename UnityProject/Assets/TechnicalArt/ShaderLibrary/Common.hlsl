// Billboard变换函数
float4 BillboardTransform(float3 positionOS, float4 color, float2 uv, out float3 worldPos)
{
	// 获取物体中心点在世界空间的位置
	float3 centerWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

	// 获取摄像机的方向和上方向
	float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - centerWorld);
	float3 up = float3(0, 1, 0);

	// 根据Billboard类型调整
#if defined(_BILLBOARDTYPE_VERTICAL)
	// 垂直Billboard：只绕Y轴旋转
	viewDir.y = 0;
	viewDir = normalize(viewDir);
#elif defined(_BILLBOARDTYPE_HORIZONTAL)
	// 水平Billboard：保持向上
	up = float3(0, 1, 0);
#else
	// 完整Billboard：完全面向摄像机
	up = UNITY_MATRIX_V[1].xyz; // 使用摄像机的上方向
#endif

	float3 right = normalize(cross(up, viewDir));
	float3 forward = normalize(cross(right, up));

	// 获取模型的原始X轴方向
	float3 modelRight = normalize(unity_ObjectToWorld._m00_m10_m20);

	// 检测是否需要翻转：当模型X轴与摄像机右向量的点积小于0时翻转
	float flipSign = dot(modelRight, right) >= 0 ? 1.0 : -1.0;

	// 应用原始缩放
#ifdef _PRESERVESCALE_ON
	float3 scale = float3(
		length(unity_ObjectToWorld._m00_m10_m20),
		length(unity_ObjectToWorld._m01_m11_m21),
		length(unity_ObjectToWorld._m02_m12_m22)
		);

	// 应用自然翻转
	scale.x *= flipSign;
#else
	float3 scale = float3(flipSign, 1, 1);
#endif

	// 构建Billboard顶点位置
	float3 worldPosition = centerWorld
		+ right * positionOS.x * scale.x
		+ up * positionOS.y * scale.y
		+ forward * positionOS.z * scale.z;

	worldPos = worldPosition;
	return TransformWorldToHClip(worldPosition);
}