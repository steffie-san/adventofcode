require("Utils")

function ExecuteDay1(doStar1, doStar2)

    if not doStar1 and not doStar2 then
        print("what do you want? doStar1 or doStar2 should be true when you call this function")
        return
    end

    local left = {}
    local right = {}

    for k, line in pairs(ReadInput(1)) do
        local a, b = line:match("(%d+)%s+(%d+)")

        left[k] = a
        right[k] = b
    end

    table.sort(left)
    table.sort(right)
    if doStar1 then
        local sum = 0
        for k, v in ipairs(left) do
            local a = left[k]
            local b = right[k]
            sum = sum + math.abs(a - b)
        end
        print("the output for star 1-1 ", sum)
    end


    if doStar2 then
        local similarity_score = 0

        for k_1, v_1 in ipairs(left) do
            local multiplier = 0
            for k_2, v_2 in ipairs(right) do
                if v_1 == v_2 then multiplier = multiplier + 1 end
            end
            similarity_score = similarity_score + multiplier * v_1
        end

        print("the output for star 1-2: ", similarity_score)
    end
end
