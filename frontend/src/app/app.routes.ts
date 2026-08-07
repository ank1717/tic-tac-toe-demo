import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { LandingComponent } from './components/landing/landing';
import { GameBoardComponent } from './components/game-board/game-board';

const validGameModes = ['TwoPlayer', 'AgainstComputer'];

const validateGameRoute: CanActivateFn = (route) => {
  const mode = route.queryParamMap.get('mode');
  if (mode && validGameModes.includes(mode)) {
    return true;
  }
  return inject(Router).parseUrl('/');
};

export const routes: Routes = [
  { path: '', component: LandingComponent, pathMatch: 'full' },
  { path: 'game', component: GameBoardComponent, canActivate: [validateGameRoute] },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
