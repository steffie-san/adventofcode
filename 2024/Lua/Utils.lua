function ReadInput(day)
    local lines = {}
    for line in io.lines("inputs/day_"..day..".txt") do
        lines[#lines + 1] = line
    end
    return lines
end
