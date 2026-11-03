// Recent Attendance Employee Search
const desktopSearch = document.getElementById('desktopEmployeeSearch');
const mobileSearch = document.getElementById('mobileEmployeeSearch');
const attendanceTable = document.getElementById('recentAttendanceTable');

if (attendanceTable) {

    const attendanceRows = attendanceTable.querySelectorAll('tbody tr');
    const employeeRows = Array.from(attendanceRows).filter(row =>
        row.querySelector('.employee-name')
    );

    function filterAttendance(searchText) {

        const searchValue = searchText.trim().toLowerCase();
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
}
