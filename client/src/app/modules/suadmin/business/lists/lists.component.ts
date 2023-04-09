import { Component, ViewEncapsulation } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';

import { ViewChild, AfterViewInit } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { ResultBusinessDto, ResultBusinessListDto, ResultDeleteDto } from '../business.types';
import { BusinessService } from '../business.service';
import { AddFormComponent } from '../add-form/add-form.component';


@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})

export class ListsComponent {
  searchInputControl: UntypedFormControl = new UntypedFormControl();
  isLoading: boolean = false;
  allBusiness: ResultBusinessDto;

  displayedColumns: string[] = ['id', 'subtype', 'name', 'status', 'createdAt', 'action'];

  itemPerPage = 30;
  initialSkip = 0;
  initialLimit = this.itemPerPage
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    public dialog: MatDialog,
    public _businessService: BusinessService
  ) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    this.isLoading = true

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this._businessService!.getAllBusiness(
            this.initialSkip,
            this.initialLimit,
            "desc"
          ).pipe(catchError(() => observableOf(null)));
        }),
        map((data: ResultBusinessListDto) => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = data.data === null;

          if (this.initialLimit >= data.total) {
            this.initialSkip = 0
            this.initialLimit = this.itemPerPage
          } else {
            this.initialSkip = data.limit
            this.initialLimit = data.limit + this.itemPerPage
          }

          if (data.data === null) {
            return [];
          }

          this.resultsLength = data.total;
          return data;
        }),
      )
      .subscribe((data: ResultBusinessListDto) => {
        this.allBusiness = data.data;
        this.isLoading = false
      });
  }

  getAllTypes(): void {
    this.isLoading = true
    this._businessService.getAllBusiness(0, this.initialLimit, "desc").pipe(
      map((data: ResultBusinessListDto) => {
        this.isLoadingResults = false;
        this.isRateLimitReached = data === null;

        if (data.data === null) {
          return [];
        }
        return data;
      })
    ).subscribe((result: ResultBusinessListDto) => {
      this.initialSkip = 0;
      this.initialLimit = this.itemPerPage;
      this.resultsLength = result.total;
      this.allBusiness = result.data
      this.resultsLength = result.total
      this.isLoading = false
    })
  }

  createProduct(): void {
    const dialogRef = this.dialog.open(AddFormComponent, {
      data: {
        currentList: this._businessService,
        defaultType: null
      }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.getAllTypes()
    });
  }

  editType(_data: ResultBusinessDto): void {
    const dialogRef = this.dialog.open(AddFormComponent, {
      data: {
        currentList: this.allBusiness,
        defaultType: _data
      }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.getAllTypes()
    });
  }

  deletetype(id: number): void {
    this.isLoading = true
    this._businessService.deleteBusiness(id).pipe(
      map((data: ResultDeleteDto) => {
        this.getAllTypes() 
        return data
      })
    ).subscribe(
      (result: ResultDeleteDto) => {
        this.isLoading = false
        if(result.status){
          
        }
      }
    )
  }
}