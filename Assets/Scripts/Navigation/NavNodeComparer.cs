using System.Collections;
using System.Collections.Generic;

public class NavNodeComparer : IComparer<NavNode> {
  public int Compare(NavNode a, NavNode b) {
    if(a == b) {
      return 0;
    } else if(b == null) {
      return -1;
    } else if(a == null) {
      return 1;
    }

    int compare = a.totalCost.CompareTo(b.totalCost);
    if (compare == 0) {
      compare = a.toEndCost.CompareTo(b.toEndCost);
    }
    return compare;
  }
}