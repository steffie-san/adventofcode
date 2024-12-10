function ReadInput(day)
    local lines = {}
    for line in io.lines("inputs/day_"..day..".txt") do
        lines[#lines + 1] = line
    end
    return lines
end

function CountEntries(table)
    local count = 0
    for _ in pairs(table) do
        count = count + 1
    end
    return count
end

function ArrayToString(table)
    local result = ""
    for _,v in ipairs(table) do
        result = result .. v .. ","
    end
    return result
end