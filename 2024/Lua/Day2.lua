require("Utils")

local function IsLevelDiffSafe(level, previous)
    local diff = math.abs(level - previous)
    return diff > 0 and diff <= 3
end

local function IsLevelSafe(level, previous, shouldBeIncreasing)
    return IsLevelDiffSafe(level, previous) and (level > previous) == shouldBeIncreasing
end

local function IsReportSafe(report)
    local increasing = true
    local safe = true
    for index, level in ipairs(report) do
        if index > 1 then
            local previous = report[index - 1]
            if index == 2 then
                increasing = level > previous
                safe = IsLevelDiffSafe(level, previous)
            else
                safe = IsLevelSafe(level, previous, increasing)
            end
            if not safe then
                break
            end
        end
    end
    return safe
end

function ExecuteDay2(doStar1, doStar2)
    local input = ReadInput(2)
    local reports = {}
    for _, raw_report in pairs(input) do
        local report = {}
        for str in string.gmatch(raw_report, "([%S]+)") do
            table.insert(report, tonumber(str))
        end
        table.insert(reports, report)
    end

    local safecount = 0
    local secondSafeCount = 0

    for _, report in ipairs(reports) do
        local safe = IsReportSafe(report)
        if safe then
            safecount = safecount + 1
        elseif doStar2 then
            for i, _ in ipairs(report) do
                local reportCopy = {}
                for j, level in ipairs(report) do
                    if i ~= j then
                        table.insert(reportCopy, level)
                    end
                end
                safe = IsReportSafe(reportCopy)
                if safe then
                    break
                end
            end
            if safe then
                secondSafeCount = secondSafeCount + 1
            end
        end
    end
    if doStar1 then
        print("The answer for star 2-1 is", safecount)
    end
    if doStar2 then
        print("The answer for star 2-2 is", (safecount + secondSafeCount))
    end
end
