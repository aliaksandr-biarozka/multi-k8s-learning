using System.Collections.Generic;
using System;

  public class ListNode {
      public int val;
      public ListNode next;
      public ListNode(int val=0, ListNode next=null) {
          this.val = val;
          this.next = next;
      }
  }
 
public class Solution {
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2) {
        ListNode head = new ListNode(1.val + l2.val >= 10 ? l1.val + l2.val - 10 : l1.val + l2.val);
        ListNode current = head;
        var addon = 1.val + l2.val >= 10 ? 1 : 0;
        l1 = l1.next;
        l2 = l2.next;
        while(l1 != null || l2 != null) {
            var sum = (l1?.val ?? 0) + (l2?.val ?? 0) + addon;
            if(sum < 10) {
                current.next = new ListNode(sum);
                addon = 0;
            } else {
                current.next = new ListNode(sum - 10);
                addon = 1;
            }

            current = current.next;
            l1 = l1?.next;
            l2 = l2?.next;
        }

        if(addon != 0) {
            current.next = new ListNode(addon);
        }

        return head;
    }

    public IEnumerable<int> GetIntersection(int[] a, int[] b) {
        int i = 0, j = 0;
        var result = new List<int>();
        while(i < a.Length && j < b.Length) {
            if(a[i] == b[j]) {
                result.Add(a[i]);
            } else if(a[i] > b[j]) {
                j++;
            } else {
                i++;
            }
        }

        return result;
    }

    public int GetLongestSubStringLength(string s) {
        int maxLength = 0, i = 0, lastUniqueLetter = 0;;
        var wordLetters = new Dictionary<char, int>();

        while(i < s.Length) {
            if(!wordLetters.Contains(s[i])) {
                wordLetters.Add(s[i], i++);
            } else {
                maxLength = Math.Max(maxLength, wordLetters.Count - lastUniqueLetter);

                lastUniqueLetter = wordLetters[s[i]] + 1;
                wordLetters[s[i]] = i;
            }
        }
        
        if(maxLength < wordLetters.Count) {
            maxLength = wordLetters.Count;
        }

        return maxLength;
    }
}