require("Utils")


function ExecuteDay3(doStar1, doStar2)
    local input = ReadInput(3)
    local count = 0
    local sum = 0
    for k, v in ipairs(input) do
        print("doint line " .. k)
        local offset = 0
        while true do
            local start
            local stop
            local left
            local right
            start, stop, left, right = string.find(v, "mul%((%d%d?%d?)%,(%d%d?%d?)%)", offset)
            print(start, stop, left, right)
            if start ~= nil and stop ~= nil then
                offset = stop
                count = count + 1
                left = tonumber(left)
                right = tonumber(right)
                sum = sum + left * right
            end
            if start == nil then break end

            if stop ~= nil then
                offset = stop
            end
        end
    end
    print(count)
    print("The answer for star 3-1 is", sum)
end
