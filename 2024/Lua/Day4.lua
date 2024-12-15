require("Utils")

local function GetCellValue(input, x, y)
    return string.sub(input[y], x, x)
end

local star_1_dirs = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } }

local function AddVector(vector, x, y)
    return vector[1] + x, vector[2] + y
end

local function IsStringMatching(input, rows, columns, x, y, dir, target)
    local len = string.len(target)
    if (dir[1] == -1 and x < len) or (dir[1] == 1 and x > columns - len + 1) or (dir[2] == -1 and y < len) or (dir[2] == 1 and y > rows - len + 1) then
        return false
    end

    for i = 1, len do
        if (GetCellValue(input, x, y) ~= string.sub(target, i, i)) then
            return false
        end
        x, y = AddVector(dir, x, y)
    end
    return true
end

local function CountXMASesAt(input, rows, columns, x_r, y_r)
    local count = 0
    if GetCellValue(input, x_r, y_r) == "X" then
        for _, dir in ipairs(star_1_dirs) do
            if IsStringMatching(input, rows, columns, x_r, y_r, dir, "XMAS") then
                count = count + 1
            end
        end
    end
    return count
end

function ExecuteDay4(doStar1, doStar2)
    local input = ReadInput(4)
    local rows = CountEntries(input)
    local columns = string.len(input[1])
    local count_star_1 = 0
    for y_r = 1, rows do
        for x_r = 1, columns do
            count_star_1 = count_star_1 + CountXMASesAt(input, rows, columns, x_r, y_r)
        end
    end

    -- 1583 is too low for star 4-1
    -- 2436 is too low for star 4-1

    if doStar1 then print("The answer for star 4-1 is", count_star_1) end
    -- if doStar2 then print("The answer for star 3-2 is", secondSum) end
end
