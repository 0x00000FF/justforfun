" vim-linecount :: How many lines did you code today?
" by P.Knowledge, a.k.a 0x00000FF
"

let s:date = strftime('%y%m%d') 
let s:basePath = expand('~/.vim/bundle/vim-linecount/scores/')
let s:path = s:basePath . s:date

let s:currentBuffer = 0
let s:bufflist = []

function! s:CreateScoreFile(date)
    call system('echo 0 > ' . s:path)
endfunction

function! s:AddBuffer()
    call insert(s:bufflist, line('$'))
endfunction

function! s:DeleteBuffer()
    if !bufexists( bufnr('%') )
        return
    endif

    try
        call remove(s:bufflist, s:currentBuffer )
    catch
    endtry
endfunction

function! s:Initialize()
    let l:bufId = 0
    
    while l:bufId <= bufnr('$')
        call insert(s:bufflist, 0)
        let l:bufId += l:bufId + 1
    endwhile

    call s:ChangeBuffer()
endfunction

function! s:ChangeBuffer()
    let s:currentBuffer = bufnr('%') - 1
    
    try 
        let s:bufflist[ s:currentBuffer ] = line('$')
    catch 
    endtry
endfunction

function! s:UpdateCount()
    let l:currentLines = line('$')
    let l:diff = l:currentLines - get(s:bufflist, s:currentBuffer)
    
    write

    let s:bufflist[ s:currentBuffer ] = l:currentLines
    
    if l:diff < 0
        return
    endif
    
    let l:todayCount = str2nr(s:todayCount[0])
    let l:todayCount += l:diff

    let s:todayCount[0] = l:todayCount
    call writefile(s:todayCount, s:path)
endfunction

" ----------------------------------------------------------------------------
function s:Message(lines, ...)
    let l:msg = 'You coded ' . a:lines . ' line(s)'
    
    if a:0 == 0
        let l:msg = l:msg . ' today!'
    elseif a:1 == -1
        let l:msg = l:msg . ' so far!'
    else
        let l:msg = l:msg . ' at ' . a:1
    endif

    return l:msg
endfunction

function s:GetTotalLines()
    let l:recList = split( globpath(s:basePath, '*') , '\n' )
    let l:totalCount = 0

    for l:rec in l:recList
        let l:record = readfile(l:rec)
        let l:totalCount += str2nr(l:record[0])
    endfor

    echo s:Message(l:totalCount, -1)
endfunction

function s:GetLinesAt(dateAt)
    if !filereadable(s:basePath . a:dateAt)
        echo 'No record at that time!'
        return
    endif

    let l:record = readfile(s:basePath . a:dateAt)
    echo s:Message(get(l:record, 0), a:dateAt) 
endfunction

" ----------------------------------------------------------------------------
if !empty(s:basePath)
    call system('mkdir ' . s:basePath)
endif

if !filereadable(s:path)
    call s:CreateScoreFile(s:date)
endif

let s:todayCount = readfile(s:path)

command!                    LinesToday      echo s:Message(s:todayCount[0])
command!                    LinesTotal      call s:GetTotalLines()
command!     -nargs=1       LinesAt         call s:GetLinesAt(<q-args>)

autocmd     VimEnter        *  call s:Initialize() 
autocmd     BufAdd          *  call s:AddBuffer()
autocmd     BufDelete       *  call s:DeleteBuffer()
autocmd     BufEnter        *  call s:ChangeBuffer()
autocmd     BufWriteCmd     *  call s:UpdateCount()
