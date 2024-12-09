require("Utils")

function ExecuteDay2(doStar1, doStar2)
    local input = ReadInput(2)
    local reports = {}
    for levelIndex, raw_report in pairs(input) do
        local report = {}
        for str in string.gmatch(raw_report, "([%S]+)") do
            table.insert(report, str)
        end
        table.insert(reports, report)
    end

    local safecount = 0

    for k, report in ipairs(reports) do
        local safe = true
        local increasing = true

        for index, level in ipairs(report) do
            if index > 1 then
                local previous = report[index - 1]
                local diff = math.abs(level - previous)
                if diff > 3 or diff < 1 then
                    safe = false
                    break
                end
                if index == 2 then
                    increasing = level > previous
                elseif not ((level > previous) == increasing) then
                    safe = false
                    break
                end
            end
        end
        if safe then
            safecount = safecount + 1
        end
    end
    print("the answer for star 2-1 is: ", safecount)
end
