
;Test.get_testIndexCount()
L0000: mov eax, 0xf4240
L0005: ret

;Test.get_indexRange()
L0000: mov eax, 0x32
L0005: ret

;Test.get_rng()
L0000: sub rsp, 0x28
L0004: mov rcx, 0x7ff9461bcdc8
L000e: xor edx, edx
L0010: call 0x00007ff9a3aad5d0
L0015: mov rax, [rax]
L0018: add rsp, 0x28
L001c: ret

;Test.get_testIndexes()
L0000: sub rsp, 0x28
L0004: mov rcx, 0x7ff9461bcdc8
L000e: xor edx, edx
L0010: call 0x00007ff9a3aad5d0
L0015: mov rax, [rax+8]
L0019: add rsp, 0x28
L001d: ret

;Test.test()
L0000: push rdi
L0001: push rsi
L0002: sub rsp, 0x28
L0006: xor esi, esi
L0008: xor edi, edi
L000a: mov rcx, 0x7ff9461bcdc8
L0014: xor edx, edx
L0016: call 0x00007ff9a3aad5d0
L001b: mov rcx, [rax+8]
L001f: cmp dword ptr [rcx+8], 0
L0023: jle short L0064
L0025: mov rcx, [rax+8]
L0029: cmp edi, [rcx+8]
L002c: jae short L006e
L002e: movsxd rdx, edi
L0031: mov ecx, [rcx+rdx*4+0x10]
L0035: mov edx, 1
L003a: shl rdx, cl
L003d: mov rcx, rdx
L0040: mov r8, rsi
L0043: and r8, rcx
L0046: cmp r8, rcx
L0049: jne short L0053
L004b: not rdx
L004e: and rsi, rdx
L0051: jmp short L0059
L0053: or rdx, rsi
L0056: mov rsi, rdx
L0059: inc edi
L005b: mov rdx, [rax+8]
L005f: cmp [rdx+8], edi
L0062: jg short L0025
L0064: mov rax, rsi
L0067: add rsp, 0x28
L006b: pop rsi
L006c: pop rdi
L006d: ret
L006e: call 0x00007ff9a3aaec10
L0073: int3

;Test+Int64Tracker.Init()
L0000: xor eax, eax
L0002: ret

;Test+Int64Tracker.IsSet(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: mov rdx, r8
L0011: and rdx, [rax]
L0014: cmp rdx, r8
L0017: sete al
L001a: movzx eax, al
L001d: ret

;Test+Int64Tracker.Set(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: or r8, [rax]
L0011: mov [rax], r8
L0014: ret

;Test+Int64Tracker.UnSet(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: mov rdx, r8
L0011: not rdx
L0014: and rdx, [rax]
L0017: mov [rax], rdx
L001a: ret

;Test+Int64Tracker.Init()
L0000: xor eax, eax
L0002: ret

;Test+Int64Tracker.IsSet(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: mov rdx, r8
L0011: and rdx, [rax]
L0014: cmp rdx, r8
L0017: sete al
L001a: movzx eax, al
L001d: ret

;Test+Int64Tracker.Set(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: or r8, [rax]
L0011: mov [rax], r8
L0014: ret

;Test+Int64Tracker.UnSet(Int32)
L0000: mov rax, rcx
L0003: mov r8d, 1
L0009: mov ecx, edx
L000b: shl r8, cl
L000e: mov rdx, r8
L0011: not rdx
L0014: and rdx, [rax]
L0017: mov [rax], rdx
L001a: ret
