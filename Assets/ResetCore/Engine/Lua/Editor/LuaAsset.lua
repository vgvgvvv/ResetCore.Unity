--导入常用命名空间
luanet.load_assembly('UnityEngine')
luanet.load_assembly('Assembly-CSharp')
--导入常用类
Debug = luanet.import_type('UnityEngine.Debug')

local #NAME# = {}

function #NAME#:Awake( gameObject )
	Debug.Log(gameObject.name.." Awake")
end

function #NAME#:Start( gameObject )
	Debug.Log(gameObject.name.." Start")
end

function #NAME#:Update( gameObject )

end

return #NAME#
