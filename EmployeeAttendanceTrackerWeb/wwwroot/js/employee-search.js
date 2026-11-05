// Employee Search - Today's Attendance and Attendance Report
const desktopSearch = document.getElementById('desktopEmployeeSearch');
const mobileSearch = document.getElementById('mobileEmployeeSearch');

// Support both attendance tables.
const attendanceTables = [
    document.getElementById('recentAttendanceTable'),
    document.getElementById('attendanceReportTable')
].filter(table => table !== null);

// Filter employees in the available tables.
function filterAttendance(searchText) {

    const searchValue = searchText.trim().toLowerCase();

    attendanceTables.forEach(attendanceTable => {

        const attendanceRows = attendanceTable.querySelectorAll('tbody tr');

        const employeeRows = Array.from(attendanceRows).filter(row =>
            row.querySelector('.employee-name')
        );

        let visibleCount = 0;

        employeeRows.forEach(row => {

            const employeeName = row
                .querySelector('.employee-name')
                .textContent
                .trim()
                .toLowerCase();

            const isMatch = employeeName.includes(searchValue);

            row.style.display = isMatch ? '' : 'none';

            if (isMatch) {
                visibleCount++;
            }
        });

        // Display a message when no employee matches.
        let noResultsRow = attendanceTable.querySelector('.no-search-results');

        if (visibleCount === 0 && employeeRows.length > 0) {

            if (!noResultsRow) {
                noResultsRow = document.createElement('tr');
                noResultsRow.className = 'no-search-results';

                noResultsRow.innerHTML = `
                    <td colspan="5" class="text-center text-muted py-4">
                        No matching employees found.
                    </td>
                `;

                attendanceTable.querySelector('tbody').appendChild(noResultsRow);
            }

            noResultsRow.style.display = '';
        }
        else if (noResultsRow) {
            noResultsRow.remove();
        }
    });
}

// Desktop search
if (desktopSearch) {
    desktopSearch.addEventListener('input', function () {

        filterAttendance(this.value);

        if (mobileSearch) {
            mobileSearch.value = this.value;
        }
    });
}

// Mobile search
if (mobileSearch) {
    mobileSearch.addEventListener('input', function () {

        filterAttendance(this.value);

        if (desktopSearch) {
            desktopSearch.value = this.value;
        }
    });
}
