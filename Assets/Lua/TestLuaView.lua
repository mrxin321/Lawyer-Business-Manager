local speed = 10;
local lightCpnt = nil;

function start()
    print("lua start...");
    print("injected object", lightObject);
    -- lightCpnt = lightObject:GetComponent(typeof(CS.UnityEngine.Light))

    print("self", self);
    print("btn1", btn1);
    print("CS.TestLuaView", CS.TestLuaView);
    print("CS", CS);

    audio:GetComponent("AudioSource"):Play();

    btn1:GetComponent("Button").onClick:AddListener(function()
        print("你点击了这个按钮！！！！！！！！！！！");
    end)

    -- self.StaticLuaTestUse();

    -- self:LuaTestUse();

    -- CS.TestLuaView.LuaTestUse();

    -- CS.TestLuaView.StaticLuaTestUse();
end

function update()
    -- print("执行update");
	local r = CS.UnityEngine.Vector3.up * CS.UnityEngine.Time.deltaTime * speed
	self.transform:Rotate(r)

    btn1.transform:Rotate(r);
	-- lightCpnt.color = CS.UnityEngine.Color(CS.UnityEngine.Mathf.Sin(CS.UnityEngine.Time.time) / 2 + 0.5, 0, 0, 1)
    -- local str = r:tostr
    -- print("r:-----------");
end

function ondestroy()
    print("lua destroy")
end