import React from 'react';
import classes from './Film.module.css';

import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import Button from '../../components/Button/Button';
import Modal from '@mui/material/Modal';
import ReactPlayer from 'react-player/youtube';
import CircularProgress from '@mui/material/CircularProgress';

import posterUrl from './posters/spider_man_no_way_home.jpg';
import featuredImageUrl from './featured-images/spider_man_no_way_home.jpg';

const films = {
    spiderman: {
        title: 'Spider-Man: No Way Home',
        description: `For the first time in the cinematic history of Spider-Man,
        our friendly neighborhood hero's identity is revealed, 
        bringing his Super Hero responsibilities into conflict with 
        is normal life and putting those he cares about most at risk.
        When he enlists Doctor Strange's help to restore his secret,
        the spell tears a hole in their world,
        releasing the most powerful villains who've ever fought a Spider-Man in any universe.
        Now, Peter will have to overcome his greatest challenge yet,
        which will not only forever alter his own future but the future of the Multiverse.`,
        imdbRating: 8.1,
        genre: 'Action',
        duration: '1h 51m',
        airLanguage: 'English',
        posterUrl: posterUrl,
        featuredImageUrl: featuredImageUrl,
        notableStaff: [
            {
                picture: 'https://i.pravatar.cc?img=1',
                title: 'Directed by',
                description: 'Jonathan Liebesman',
            },
            {
                picture: 'https://i.pravatar.cc?img=2',
                title: 'Produced by',
                description: 'Basil Iwanyk',
            },
            {
                picture: 'https://i.pravatar.cc?img=3',
                title: 'Screenplay by',
                description: 'Savid Leslie Johnson',
            },
            {
                picture: 'https://i.pravatar.cc?img=4',
                title: 'Based on',
                description: 'Characters by Beverley Cross',
            },
            {
                picture: 'https://i.pravatar.cc?img=5',
                title: 'Main Actor',
                description: 'Sam Washington',
            },
        ],
        trailerUrl: 'https://www.youtube.com/embed/JfVOs4VSpmA',
    },
};

const Film = (props) => {
    const params = useParams();
    const filmId = 'spiderman'; //params.filmId;
    const film = films[filmId];

    const [isShowingTrailerDialog, setIsShowingTrailerDialog] = useState(false);
    const openTrailerDialog = () => {
        setIsShowingTrailerDialog(true);
    };
    const closeTrailerDialog = () => {
        setIsShowingTrailerDialog(false);
    };

    const [isTrailerLoaded, setIsTrailerLoaded] = useState(false);

    return (
        <React.Fragment>
            <Modal
                open={isShowingTrailerDialog}
                onClose={closeTrailerDialog}
                className={classes['responsive-trailer-modal']}
            >
                <div className={classes['responsive-trailer-wrapper']}>
                    {!isTrailerLoaded && (
                        <div
                            className={
                                classes['responsive-trailer-loading-overlay']
                            }
                        >
                            <CircularProgress
                                className={
                                    classes[
                                        'responsive-trailer-loading-spinner'
                                    ]
                                }
                            />
                            Loading...
                        </div>
                    )}
                    <ReactPlayer
                        className={classes['responsive-trailer-player']}
                        url={film.trailerUrl}
                        controls={true}
                        playing={false}
                        onReady={() => setIsTrailerLoaded(true)}
                    />
                </div>
            </Modal>
            <div
                className={classes['film-container']}
                style={{
                    '--background-image-url': `url(${film.featuredImageUrl})`,
                }}
            >
                <div className={classes['film-container-content']}>
                    <div className={classes['film-details']}>
                        <div className={classes['film-metadata']}>
                            <div className={classes['film-imdb-rating']}>
                                <img src="https://m.media-amazon.com/images/G/01/IMDb/BG_rectangle._CB1509060989_SY230_SX307_AL_.png" />
                                <span>{film.imdbRating} / 10</span>
                            </div>
                            <div>{film.genre}</div>
                            <div>{film.duration}</div>
                            <div>{film.airLanguage}</div>
                        </div>

                        <h1 className={classes['film-title']}>
                            <strong>{film.title}</strong>
                        </h1>

                        <p className={classes['film-description']}>
                            {film.description}
                        </p>

                        <div className={classes['film-action-buttons']}>
                            <Button onClick={openTrailerDialog}>
                                Watch Trailer
                            </Button>

                            <Link to="book">
                                <Button variant="alternative">
                                    Order Tickets
                                </Button>
                            </Link>
                        </div>
                    </div>

                    <div className={classes['film-poster-container']}>
                        <img
                            src={posterUrl}
                            className={classes['film-poster']}
                        />
                    </div>

                    <div className={classes['film-notable-staff-container']}>
                        {film.notableStaff.map(
                            (
                                notableStaff,
                                notableStaffIndex,
                                notableStaffCollection
                            ) => {
                                const isLast =
                                    notableStaffIndex + 1 ===
                                    notableStaffCollection.length;

                                return (
                                    <>
                                        <div
                                            className={
                                                classes[
                                                    'film-notable-staff-item'
                                                ]
                                            }
                                            key={`notable-staff-item-${notableStaff.title}`}
                                        >
                                            <img
                                                className={
                                                    classes[
                                                        'film-notable-staff-image'
                                                    ]
                                                }
                                                src={notableStaff.picture}
                                            />
                                            <div>
                                                <div
                                                    className={
                                                        classes[
                                                            'film-notable-staff-title'
                                                        ]
                                                    }
                                                >
                                                    {notableStaff.title}
                                                </div>
                                                <div
                                                    className={
                                                        classes[
                                                            'film-notable-staff-description'
                                                        ]
                                                    }
                                                >
                                                    {notableStaff.description}
                                                </div>
                                            </div>
                                        </div>
                                        {!isLast && (
                                            <div
                                                className={
                                                    classes[
                                                        'film-notable-staff-separator'
                                                    ]
                                                }
                                                key={`notable-staff-separator-${notableStaff.title}`}
                                            ></div>
                                        )}
                                    </>
                                );
                            }
                        )}
                    </div>
                </div>
            </div>
        </React.Fragment>
    );
};

export default Film;
