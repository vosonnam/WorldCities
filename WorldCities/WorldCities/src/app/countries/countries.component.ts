import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { environment } from './../../environments/environment';
import { Country } from './country';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss']
})
export class CountriesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3'];
  public countries!: MatTableDataSource<Country>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  filterTextChange: Subject<string> = new Subject<string>();

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadData(undefined);
  }
  loadData(query?: string) {
    var e: PageEvent = new PageEvent();
    e.pageIndex = this.defaultPageIndex;
    e.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(e);

  }
  getData(e: PageEvent) {
    var query = [
      e.pageIndex.toString(),
      e.pageSize.toString(),
      (this.sort) ? this.sort.active : this.defaultSortColumn,
      (this.sort) ? this.sort.direction : this.defaultSortOrder
    ];
    if (this.filterQuery) {
      query.push(this.defaultFilterColumn);
      query.push(this.filterQuery);
    }
    var url = environment.BaseUrl + 'api/Countries/Page/' + query.join('/');
    this.http.get<any>(url).subscribe(
      result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.countries = new MatTableDataSource<Country>(result.data);
      },
      error => console.error(error)
    );
  }
  onFilterTextChange(filterText: string) {
    if (this.filterTextChange.observers.length === 0) {
      this.filterTextChange
        .pipe(
          debounceTime(1000),
          distinctUntilChanged()
        )
        .subscribe(_query => this.loadData(_query));
    }
    this.filterTextChange.next(filterText);
  }
}
