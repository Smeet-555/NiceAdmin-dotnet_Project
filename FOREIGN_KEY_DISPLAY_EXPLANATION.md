# How Foreign Key Display Names Work

## Overview
When working with foreign keys, we store only the **ID** (e.g., `DepartmentId = 5`) but need to display the **Name** (e.g., `DepartmentName = "IT"`). Here's how this works in different scenarios:

---

## Approach 1: Stored Procedure with JOIN (Recommended)

### Example: Staff List

**Stored Procedure (`PR_Staff_SelectAll`):**
```sql
SELECT 
    s.StaffID,
    s.StaffName,
    s.DepartmentID,          -- Foreign Key ID
    s.MobileNo,
    s.EmailAddress,
    s.Remarks,
    d.DepartmentName         -- Display Name from JOIN
FROM MOM_Staff s
INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
```

**Controller Code:**
```csharp
while (reader.Read())
{
    staffList.Add(new StaffModelView
    {
        StaffId = Convert.ToInt32(reader["StaffID"]),
        StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
        DepartmentId = Convert.ToInt32(reader["DepartmentID"]),      // Foreign Key
        DepartmentName = reader["DepartmentName"]?.ToString() ?? ""  // Display Name
    });
}
```

**Result:**
- The stored procedure does a JOIN to get both the ID and Name
- One database call gets everything
- Most efficient approach

---

## Approach 2: Separate Query for Display Names

### Example: Meeting Members List

**Step 1 - Get Main Data:**
```csharp
// PR_MeetingMember_SelectAll returns:
// MeetingMemberID, MeetingID, StaffID, StaffName, IsPresent, Remarks
while (reader.Read())
{
    meetingMembers.Add(new MeetingMemberViewModel
    {
        MeetingMemberId = Convert.ToInt32(reader["MeetingMemberID"]),
        StaffId = Convert.ToInt32(reader["StaffID"]),
        StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
        DepartmentName = string.Empty  // Not available yet
    });
}
```

**Step 2 - Get Missing Display Names:**
```csharp
// Loop through each member and get their department name
foreach (var member in meetingMembers)
{
    using var cmd = new SqlCommand(@"
        SELECT d.DepartmentName 
        FROM MOM_Staff s 
        INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID 
        WHERE s.StaffID = @StaffID", con);
    cmd.Parameters.AddWithValue("@StaffID", member.StaffId);
    
    var result = cmd.ExecuteScalar();
    member.DepartmentName = result?.ToString() ?? "Unknown";
}
```

**Result:**
- First query gets main data
- Additional queries get missing display names
- Less efficient (N+1 query problem) but works when stored procedure doesn't include all JOINs

---

## Approach 3: Dropdowns in Forms

### Example: Staff Add/Edit Form

**Controller - Load Dropdown Data:**
```csharp
private void LoadDropdownData()
{
    var departments = new List<object>();
    using (var cmd = new SqlCommand("PR_Department_SelectAll", con))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                // Create anonymous object with Value (ID) and Text (Name)
                departments.Add(new { 
                    Value = reader["DepartmentID"],    // Foreign Key
                    Text = reader["DepartmentName"]    // Display Name
                });
            }
        }
    }
    ViewBag.Departments = departments;
}
```

**View - Display Dropdown:**
```html
<select asp-for="DepartmentId" class="form-select" required>
    <option value="">Select Department</option>
    @if (ViewBag.Departments != null)
    {
        foreach (dynamic dept in ViewBag.Departments)
        {
            <option value="@dept.Value">@dept.Text</option>
        }
    }
</select>
```

**Result:**
- Dropdown shows "IT", "HR", "Finance" (display names)
- When submitted, form sends `DepartmentId = 5` (foreign key)
- Database stores only the ID

---

## The Complete Flow

### Adding a Staff Member:

1. **User Opens Form:**
   - Controller calls `LoadDropdownData()`
   - Loads all departments: `[{Value: 1, Text: "IT"}, {Value: 2, Text: "HR"}]`
   - Dropdown shows: "IT", "HR", etc.

2. **User Fills Form:**
   - Name: "John Doe"
   - Department: Selects "IT" from dropdown
   - Mobile: "1234567890"
   - Email: "john@example.com"

3. **Form Submission:**
   ```csharp
   // Model received by controller:
   {
       StaffId = 0,
       StaffName = "John Doe",
       DepartmentId = 1,           // Only ID is sent
       DepartmentName = "",        // Empty (not sent from form)
       MobileNo = "1234567890",
       EmailAddress = "john@example.com"
   }
   ```

4. **Save to Database:**
   ```csharp
   cmd.Parameters.AddWithValue("@StaffName", "John Doe");
   cmd.Parameters.AddWithValue("@DepartmentID", 1);  // Only ID is saved
   cmd.Parameters.AddWithValue("@MobileNo", "1234567890");
   cmd.Parameters.AddWithValue("@EmailAddress", "john@example.com");
   ```

5. **Display in List:**
   - `PR_Staff_SelectAll` does JOIN to get both ID and Name
   - Shows: "John Doe | IT | 1234567890 | john@example.com"

---

## Why This Design?

### ViewModel Has Both ID and Name:
```csharp
public class StaffModelView
{
    public int DepartmentId { get; set; }      // For saving (foreign key)
    public string DepartmentName { get; set; }  // For displaying
}
```

### Benefits:
1. **Normalization:** Database stores only IDs (no data duplication)
2. **Flexibility:** Can display names without changing database
3. **Integrity:** Foreign key constraints ensure data validity
4. **Efficiency:** JOINs in stored procedures are fast

### Validation Strategy:
- `DepartmentId` has `[Required]` - must be selected from dropdown
- `DepartmentName` has NO `[Required]` - it's just for display, not submitted

---

## Summary Table

| Scenario | Foreign Key | Display Name | How It's Populated |
|----------|-------------|--------------|-------------------|
| **List View** | `DepartmentId = 1` | `DepartmentName = "IT"` | Stored procedure with JOIN |
| **Add Form** | `DepartmentId = 0` | `DepartmentName = ""` | Not needed (dropdown shows options) |
| **Edit Form** | `DepartmentId = 1` | `DepartmentName = "IT"` | Loaded from database via JOIN |
| **Save Action** | `DepartmentId = 1` | `DepartmentName = ""` | Only ID is saved to database |

---

## Best Practices

1. **Always use JOINs in SELECT stored procedures** to get display names
2. **Only send IDs in INSERT/UPDATE** stored procedures
3. **ViewModels should have both** ID (for saving) and Name (for displaying)
4. **Validation on ID only**, not on display name
5. **Use ViewBag for dropdowns** to pass options from controller to view
