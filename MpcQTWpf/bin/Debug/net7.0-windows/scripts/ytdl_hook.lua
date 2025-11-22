local utils = require 'mp.utils'
local msg = require 'mp.msg'
local options = require 'mp.options'

local o = {
    exclude = "",
    try_ytdl_first = false,
    use_manifests = false
}
options.read_options(o)

local ytdl = {
    path = "lib\\yt-dlp.exe",   -- switched from youtube-dl.exe to yt-dlp.exe
    searched = false,
    blacklisted = {}
}

local chapter_list = {}

function Set (t)
    local set = {}
    for _, v in pairs(t) do set[v] = true end
    return set
end

local safe_protos = Set {
    "http", "https", "ftp", "ftps",
    "rtmp", "rtmps", "rtmpe", "rtmpt", "rtmpts", "rtmpte",
    "data"
}

local function exec(args)
    local ret = utils.subprocess({args = args})
    return ret.status, ret.stdout, ret, ret.killed_by_us
end

local function option_was_set(name)
    return mp.get_property_bool("option-info/" ..name.. "/set-from-commandline", false)
end

local function option_was_set_locally(name)
    return mp.get_property_bool("option-info/" ..name.. "/set-locally", false)
end

local function set_http_headers(http_headers)
    if not http_headers then return end
    local headers = {}
    local useragent = http_headers["User-Agent"]
    if useragent and not option_was_set("user-agent") then
        mp.set_property("file-local-options/user-agent", useragent)
    end
    local additional_fields = {"Cookie", "Referer", "X-Forwarded-For"}
    for _, item in pairs(additional_fields) do
        local field_value = http_headers[item]
        if field_value then
            headers[#headers + 1] = item .. ": " .. field_value
        end
    end
    if #headers > 0 and not option_was_set("http-header-fields") then
        mp.set_property_native("file-local-options/http-header-fields", headers)
    end
end

local function append_libav_opt(props, name, value)
    if not props then props = {} end
    if name and value and not props[name] then
        props[name] = value
    end
    return props
end

local function edl_escape(url)
    return "%" .. string.len(url) .. "%" .. url
end

local function url_is_safe(url)
    local proto = type(url) == "string" and url:match("^(.+)://") or nil
    local safe = proto and safe_protos[proto]
    if not safe then
        msg.error(("Ignoring potentially unsafe url: '%s'"):format(url))
    end
    return safe
end

local function time_to_secs(time_string)
    local a, b, c = time_string:match("(%d+):(%d%d?):(%d%d)")
    if a ~= nil then
        return (a*3600 + b*60 + c)
    else
        a, b = time_string:match("(%d%d?):(%d%d)")
        if a ~= nil then
            return (a*60 + b)
        end
    end
    return nil
end

local function extract_chapters(data, video_length)
    local ret = {}
    for line in data:gmatch("[^\r\n]+") do
        local time = time_to_secs(line)
        if time and (time < video_length) then
            table.insert(ret, {time = time, title = line})
        end
    end
    table.sort(ret, function(a, b) return a.time < b.time end)
    return ret
end

local function is_blacklisted(url)
    if o.exclude == "" then return false end
    if #ytdl.blacklisted == 0 then
        local joined = o.exclude
        while joined:match('%|?[^|]+') do
            local _, e, substring = joined:find('%|?([^|]+)')
            table.insert(ytdl.blacklisted, substring)
            joined = joined:sub(e+1)
        end
    end
    if #ytdl.blacklisted > 0 then
        url = url:match('https?://(.+)')
        for _, exclude in ipairs(ytdl.blacklisted) do
            if url:match(exclude) then
                msg.verbose('URL matches excluded substring. Skipping.')
                return true
            end
        end
    end
    return false
end

-- Playlist parsing, manifest handling, add_single_video, etc.
-- (all the functions you pasted earlier remain unchanged)

-- run_ytdl_hook implementation
function run_ytdl_hook(url)
    local start_time = os.clock()

    if not (ytdl.searched) then
        local exesuf = (package.config:sub(1,1) == '\\') and '.exe' or ''
        local ytdl_mcd = mp.find_config_file("yt-dlp" .. exesuf)
        if not (ytdl_mcd == nil) then
            msg.verbose("found yt-dlp at: " .. ytdl_mcd)
            ytdl.path = ytdl_mcd
        end
        ytdl.searched = true
    end

    if (url:find("ytdl://") == 1) then
        url = url:sub(8)
    end

    local format = mp.get_property("options/ytdl-format")
    local raw_options = mp.get_property_native("options/ytdl-raw-options")
    local allsubs = true
    local proxy = nil
    local use_playlist = false

    local command = {
        ytdl.path, "--no-warnings", "-J", "--flat-playlist",
        "--sub-format", "ass/srt/best"
    }

    if (mp.get_property("options/vid") == "no")
        and not option_was_set("ytdl-format") then
        format = "bestaudio/best"
        msg.verbose("Video disabled. Only using audio")
    end

    if (format == "") then
        format = "bestvideo+bestaudio/best"
    end
    table.insert(command, "--format")
    table.insert(command, format)

    for param, arg in pairs(raw_options) do
        table.insert(command, "--" .. param)
        if (arg ~= "") then
            table.insert(command, arg)
        end
        if (param == "sub-lang") and (arg ~= "") then
            allsubs = false
        elseif (param == "proxy") and (arg ~= "") then
            proxy = arg
        elseif (param == "yes-playlist") then
            use_playlist = true
        end
    end

    if (allsubs == true) then
        table.insert(command, "--all-subs")
    end
    if not use_playlist then
        table.insert(command, "--no-playlist")
    end
    table.insert(command, "--")
    table.insert(command, url)
    msg.debug("Running: " .. table.concat(command,' '))
    local es, json, result, aborted = exec(command)

    if aborted then return end
    if (es < 0) or (json == nil) or (json == "") then
        msg.error("yt-dlp failed")
        return
    end

    local json, err = utils.parse_json(json)
    if (json == nil) then
        msg.error("failed to parse JSON data: " .. err)
        return
    end

    msg.verbose("yt-dlp succeeded!")
    msg.debug('ytdl parsing took '..os.clock()-start_time..' seconds')

    json["proxy"] = json["proxy"] or proxy

    -- playlist vs single video handling (calls add_single_video, etc.)
    -- (rest of your pasted logic continues here unchanged)
end

-- Hook registration
if (not o.try_ytdl_first) then
    mp.add_hook("on_load", 10, function ()
        msg.verbose('ytdl:// hook')
        local url = mp.get_property("stream-open-filename", "")
        if not (url:find("ytdl://") == 1) then
            msg.verbose('not a ytdl:// url')
            return
        end
        run_ytdl_hook(url)
    end)
end

mp.add_hook(o.try_ytdl_first and "on_load" or "on_load_fail", 10, function()
    msg.verbose('full hook')
    local url = mp.get_property("stream-open-filename", "")
    if not (url:find("ytdl://") == 1) and
        not ((url:find("https?://") == 1) and not is_blacklisted(url)) then
        return
    end
    run_ytdl_hook(url)
end)

mp.add_hook("on_preloaded", 10, function ()
    -- optional: handle subtitles/chapters after preload
end)