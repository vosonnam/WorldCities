import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { environment } from './../../environments/environment';
import { City } from './city';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})
export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;

  defaultpageIndex: number = 0;
  defaultpageSize: number = 10;
  public defaultSortColumn: string = 'name';
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChange:Subject<string>=new Subject<string>()

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadData(undefined);
  }

  loadData(query?:string) {
    var e: PageEvent = new PageEvent();
    e.pageIndex = 0;
    e.pageSize = 10;
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
    var url = environment.BaseUrl + 'api/Cities/Page/' + query.join('/');
    this.http.get<any>(url).subscribe(
      result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.cities = new MatTableDataSource<City>(result.data);
      },
      error => console.error(error)
    );
  }

  onFilterTextChange(filterText:string) {
    if (this.filterTextChange.observers.length === 0) {
      this.filterTextChange
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(_query => this.loadData(_query));
    }
    this.filterTextChange.next(filterText);
  }

}
