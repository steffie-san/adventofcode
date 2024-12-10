require("Utils")

local function GetCellValue(input, x, y)
    return string.sub(input[y], x, x)
end

local directions = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } }

local function AddVector(vector, x, y)
    return vector[1] + x, vector[2] + y
end

function ExecuteDay4(doStar1, doStar2)
    local input = ReadInput(4)
    local rows = CountEntries(input)
    local columns = string.len(input[1])
    local count = 0
    for x_r = 1, columns do
        for y_r = 1, rows do
            local x = x_r
            local y = y_r
            if GetCellValue(input, x, y) == "X" then
                for _, dir in ipairs(directions) do
                    if (dir[1] == -1 and x < 4) or (dir[1] == 1 and x > columns - 4) or (dir[2] == -1 and y < 4) or (dir[2] == 1 and y > rows - 4) then
                        goto continue
                    end
                    x, y = AddVector(dir, x, y)
                    if GetCellValue(input, x, y) == "M" then
                        x, y = AddVector(dir, x, y)
                        if GetCellValue(input, x, y) == "A" then
                            x, y = AddVector(dir, x, y)
                            if GetCellValue(input, x, y) == "S" then
                                count = count + 1
                            end
                        end
                    end
                    ::continue::
                end
            end
        end
    end

    -- 1583 is too low for star 4-1

    if doStar1 then print("The answer for star 4-1 is", count) end
    -- if doStar2 then print("The answer for star 3-2 is", secondSum) end
end
