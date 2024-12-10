require("Utils")


function ExecuteDay3(doStar1, doStar2)
    local input = ReadInput(3)
    local sum = 0
    local secondSum = 0
    local doProcess = true
    for k, v in ipairs(input) do
        print("doint line " .. k)
        local offset = 0
        while true do
            local start
            local stop
            local left
            local right
            start, stop, left, right = string.find(v, "mul%((%d%d?%d?)%,(%d%d?%d?)%)", offset)

            local nextDo = string.find(v, "do%(%)", offset)
            local nextDont = string.find(v, "don%'t%(%)", offset)

            if start == nil then
                break
            end

            if nextDo ~= nil and nextDo < start then
                if nextDont ~= nil and nextDont < nextDo then
                    doProcess = false
                    offset = nextDont + 1
                    goto continue
                end
                doProcess = true
                offset = nextDo + 1
                goto continue
            end
            if nextDont ~= nil and nextDont < start then
                doProcess = false
                offset = nextDont + 1
                goto continue
            end

            if start ~= nil and stop ~= nil then
                offset = stop
                left = tonumber(left)
                right = tonumber(right)
                local toAdd = left * right
                sum = sum + toAdd
                if doProcess then
                    secondSum = secondSum + toAdd
                end
            end
            ::continue::
        end
    end
    if doStar1 then print("The answer for star 3-1 is", sum) end
    if doStar2 then print("The answer for star 3-2 is", secondSum) end
end
